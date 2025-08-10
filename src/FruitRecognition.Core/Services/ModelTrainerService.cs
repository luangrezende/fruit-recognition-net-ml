using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Vision;
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
        _logger.LogInformation("Starting ResNetV2101 model training with {Count} samples", trainingData.Length);

        var mlContext = new MLContext(config.Seed ?? Environment.ProcessorCount);
        var data = mlContext.Data.LoadFromEnumerable(trainingData);
        
        var trainTestData = mlContext.Data.TrainTestSplit(data, testFraction: config.TestFraction + config.ValidationFraction, seed: config.Seed);
        var trainData = trainTestData.TrainSet;
        var tempTestData = trainTestData.TestSet;
        
        var validationTestData = mlContext.Data.TrainTestSplit(tempTestData, testFraction: config.TestFraction / (config.TestFraction + config.ValidationFraction), seed: config.Seed);
        var validationData = validationTestData.TrainSet;
        var testData = validationTestData.TestSet;

        var baseImagePath = GetBaseImagePath(trainingData);
        _logger.LogInformation("Using base image path: {BasePath}", baseImagePath);

        var options = new ImageClassificationTrainer.Options()
        {
            FeatureColumnName = "Image",
            LabelColumnName = "Label",
            ValidationSet = validationData,
            Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
            TestOnTrainSet = false
        };

        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(FruitImageData.Label))
            .Append(mlContext.Transforms.LoadImages(outputColumnName: "Image", imageFolder: baseImagePath, inputColumnName: nameof(FruitImageData.ImagePath)))
            .Append(mlContext.MulticlassClassification.Trainers.ImageClassification(options))
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Label"));

        _logger.LogInformation("Training ResNetV2101 model with transfer learning...");
        var stopwatch = Stopwatch.StartNew();
        var trainedModel = pipeline.Fit(trainData);
        stopwatch.Stop();

        _logger.LogInformation("ResNetV2101 model training completed in {Seconds:F2} seconds", stopwatch.Elapsed.TotalSeconds);

        var metrics = await EvaluateModelAsync(trainedModel, testData, mlContext);
        metrics.TrainingTimeSeconds = stopwatch.Elapsed.TotalSeconds;
        metrics.TrainingSampleCount = trainingData.Length;

        return await Task.FromResult((trainedModel, metrics));
    }

    public async Task<ModelMetrics> EvaluateModelAsync(ITransformer model, IDataView testData, MLContext mlContext)
    {
        _logger.LogInformation("Evaluating ResNetV2101 model...");

        var predictions = model.Transform(testData);
        var metrics = mlContext.MulticlassClassification.Evaluate(predictions, "Label");

        var modelMetrics = new ModelMetrics
        {
            MicroAccuracy = metrics.MicroAccuracy,
            MacroAccuracy = metrics.MacroAccuracy,
            LogLoss = metrics.LogLoss,
            ConfusionMatrix = metrics.ConfusionMatrix,
            NumberOfClasses = metrics.ConfusionMatrix.NumberOfClasses
        };

        _logger.LogInformation("ResNetV2101 Evaluation Results:");
        _logger.LogInformation("  Micro Accuracy: {MicroAccuracy:F4} ({Percentage:F2}%)", modelMetrics.MicroAccuracy, modelMetrics.MicroAccuracy * 100);
        _logger.LogInformation("  Macro Accuracy: {MacroAccuracy:F4} ({Percentage:F2}%)", modelMetrics.MacroAccuracy, modelMetrics.MacroAccuracy * 100);
        _logger.LogInformation("  Log Loss: {LogLoss:F4}", modelMetrics.LogLoss);

        return await Task.FromResult(modelMetrics);
    }

    public async Task SaveModelAsync(ITransformer model, string modelPath, DataViewSchema schema)
    {
        _logger.LogInformation("Saving ResNetV2101 model to: {ModelPath}", modelPath);

        var directory = Path.GetDirectoryName(modelPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var mlContext = new MLContext();
        mlContext.Model.Save(model, schema, modelPath);

        var fileInfo = new FileInfo(modelPath);
        _logger.LogInformation("ResNetV2101 model saved successfully. File size: {Size:F2} MB", fileInfo.Length / (1024.0 * 1024.0));
        
        await Task.CompletedTask;
    }

    private string GetBaseImagePath(FruitImageData[] trainingData)
    {
        if (trainingData.Length == 0) return string.Empty;

        var firstImagePath = trainingData[0].ImagePath;
        var directory = Path.GetDirectoryName(firstImagePath);
        
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
