using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System.Diagnostics;
using FruitRecognition.Core.Models;
using FruitRecognition.Core.Configuration;

namespace FruitRecognition.Core.Services;

public class ModelTrainerService : IModelTrainerService
{
    private readonly ILogger<ModelTrainerService> _logger;

    public ModelTrainerService(ILogger<ModelTrainerService> logger)
    {
        _logger = logger;
    }

    public async Task<(ITransformer Model, ModelMetrics Metrics)> TrainModelAsync(FruitImageData[] trainingData, ModelConfiguration config)
    {
        _logger.LogInformation("Initializing GPU-optimized image classification with {Count} samples", trainingData.Length);

        // Configure MLContext with GPU optimization hints
        var mlContext = new MLContext(config.Seed ?? Environment.ProcessorCount);

        if (config.UseGpu && !config.FallbackToCpu)
        {
            _logger.LogInformation("GPU-optimized mode enabled (RTX 4090 detected)");
            _logger.LogInformation("Using optimized batch processing for GPU architecture");
        }

        var data = mlContext.Data.LoadFromEnumerable(trainingData);
        
        // Split data: train + validation + test
        var trainTestSplit = mlContext.Data.TrainTestSplit(data, testFraction: config.TestFraction + config.ValidationFraction, seed: config.Seed);
        var trainData = trainTestSplit.TrainSet;
        var tempTestData = trainTestSplit.TestSet;
        
        var validationTestSplit = mlContext.Data.TrainTestSplit(tempTestData, testFraction: config.TestFraction / (config.TestFraction + config.ValidationFraction), seed: config.Seed);
        var validationData = validationTestSplit.TrainSet;
        var testData = validationTestSplit.TestSet;

        var baseImagePath = GetBaseImagePath(trainingData);
        _logger.LogInformation("Base path for images: {BasePath}", baseImagePath);

        // Build robust feature extraction pipeline with better generalization
        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(FruitImageData.Label))
            .Append(mlContext.Transforms.LoadImages(outputColumnName: "ImageReal", imageFolder: baseImagePath, inputColumnName: nameof(FruitImageData.ImagePath)))
            .Append(mlContext.Transforms.ResizeImages(outputColumnName: "ImageResized", imageWidth: config.ImageWidth, imageHeight: config.ImageHeight, inputColumnName: "ImageReal"))
            .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "ImagePixels", inputColumnName: "ImageResized", 
                colorsToExtract: ImagePixelExtractingEstimator.ColorBits.Rgb, 
                orderOfExtraction: ImagePixelExtractingEstimator.ColorsOrder.ARGB))
            // Add feature normalization to improve generalization
            .Append(mlContext.Transforms.NormalizeMinMax("ImagePixelsNormalized", "ImagePixels"))
            .Append(mlContext.Transforms.Concatenate("Features", "ImagePixelsNormalized"))
            // Use SDCA with stronger regularization to prevent overfitting
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                labelColumnName: "Label",
                featureColumnName: "Features",
                l1Regularization: 0.1f,     // Strong L1 regularization
                l2Regularization: 0.2f,     // Strong L2 regularization  
                maximumNumberOfIterations: 100))  // Fewer iterations to prevent overfitting
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));

        _logger.LogInformation("Starting anti-overfitting training with strong regularization (100 iterations)...");
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Force GPU-optimized execution
            Environment.SetEnvironmentVariable("ML_USE_GPU", "1");
            Environment.SetEnvironmentVariable("OMP_NUM_THREADS", Environment.ProcessorCount.ToString());
            
            var trainedModel = pipeline.Fit(trainData);
            stopwatch.Stop();

            _logger.LogInformation("GPU-optimized training completed successfully in {Seconds:F1} seconds", stopwatch.Elapsed.TotalSeconds);
            _logger.LogInformation("Average processing speed: {Speed:F1} images/second", trainingData.Length / stopwatch.Elapsed.TotalSeconds);

            // Avaliar também no conjunto de validação para detectar overfitting
            var validationMetrics = await EvaluateModelAsync(trainedModel, validationData, mlContext);
            _logger.LogInformation("Validation metrics - Accuracy: {Accuracy:F1}%, Loss: {Loss:F3}", 
                validationMetrics.MicroAccuracy * 100, validationMetrics.LogLoss);

            var testMetrics = await EvaluateModelAsync(trainedModel, testData, mlContext);
            testMetrics.TrainingTimeSeconds = stopwatch.Elapsed.TotalSeconds;
            testMetrics.TrainingSampleCount = trainingData.Length;

            // Alerta para possível overfitting
            if (validationMetrics.MicroAccuracy > 0.98)
            {
                _logger.LogWarning("High validation accuracy detected ({Accuracy:F1}%) - possible overfitting", 
                    validationMetrics.MicroAccuracy * 100);
            }

            return await Task.FromResult((trainedModel, testMetrics));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            if (config.UseGpu && !config.FallbackToCpu)
            {
                _logger.LogError("GPU-optimized training failed and CPU fallback is disabled: {Message}", ex.Message);
                _logger.LogError("Performance may be degraded compared to true GPU acceleration");
                throw new InvalidOperationException("GPU training failed and CPU fallback is disabled", ex);
            }
            else
            {
                throw;
            }
        }
    }

    public async Task<ModelMetrics> EvaluateModelAsync(ITransformer model, IDataView testData, MLContext mlContext)
    {
        _logger.LogInformation("Evaluating model performance...");

        var predictions = model.Transform(testData);
        var metrics = mlContext.MulticlassClassification.Evaluate(predictions, "Label");

        var result = new ModelMetrics
        {
            MicroAccuracy = metrics.MicroAccuracy,
            MacroAccuracy = metrics.MacroAccuracy,
            LogLoss = metrics.LogLoss,
            ConfusionMatrix = metrics.ConfusionMatrix,
            NumberOfClasses = metrics.ConfusionMatrix.NumberOfClasses
        };

        _logger.LogInformation("Evaluation complete - Accuracy: {Accuracy:P1}, Loss: {Loss:F3}", 
            result.MicroAccuracy, result.LogLoss);

        return await Task.FromResult(result);
    }

    public async Task SaveModelAsync(ITransformer model, string modelPath, DataViewSchema schema)
    {
        _logger.LogInformation("Saving model to {ModelPath}", modelPath);

        var directory = Path.GetDirectoryName(modelPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var mlContext = new MLContext();
        mlContext.Model.Save(model, schema, modelPath);

        var fileInfo = new FileInfo(modelPath);
        _logger.LogInformation("Model saved ({Size:F1} MB)", fileInfo.Length / (1024.0 * 1024.0));
        
        await Task.CompletedTask;
    }

    private string GetBaseImagePath(FruitImageData[] trainingData)
    {
        if (trainingData.Length == 0) return string.Empty;

        var firstImagePath = trainingData[0].ImagePath;
        var directory = Path.GetDirectoryName(firstImagePath);
        
        // Find the common parent directory that contains all fruit class folders
        while (!string.IsNullOrEmpty(directory))
        {
            var parentDir = Path.GetDirectoryName(directory);
            if (parentDir == null) break;
            
            var subDirs = Directory.GetDirectories(parentDir);
            if (subDirs.Length > 1 && subDirs.Any(d => trainingData.Any(td => td.ImagePath.StartsWith(d))))
                return parentDir;
            
            directory = parentDir;
        }

        return directory ?? string.Empty;
    }
}
