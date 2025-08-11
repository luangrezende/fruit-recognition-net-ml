using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FruitRecognition.Core.Services;
using FruitRecognition.Core.Configuration;
using FruitRecognition.Core.Models;

namespace FruitRecognition.Training.Services;

public class TrainingService
{
    private readonly ILogger<TrainingService> _logger;
    private readonly IDataLoaderService _dataLoader;
    private readonly IModelTrainerService _modelTrainer;
    private readonly ModelConfiguration _modelConfig;
    private readonly PathConfiguration _pathConfig;

    public TrainingService(ILogger<TrainingService> logger, IDataLoaderService dataLoader, IModelTrainerService modelTrainer, IOptions<ModelConfiguration> modelConfig, IOptions<PathConfiguration> pathConfig)
    {
        _logger = logger;
        _dataLoader = dataLoader;
        _modelTrainer = modelTrainer;
        _modelConfig = modelConfig.Value;
        _pathConfig = pathConfig.Value;
    }

    public async Task RunAsync(string[] args)
    {
        _logger.LogInformation("Starting fruit recognition training...");

        try
        {
            var datasetPath = args.Length > 0 ? args[0] : _pathConfig.DatasetPath;
            var modelPath = args.Length > 1 ? args[1] : _pathConfig.ModelPath;

            datasetPath = ConvertToAbsolutePath(datasetPath);
            modelPath = ConvertToAbsolutePath(modelPath);

            _logger.LogInformation("Using dataset: {DatasetPath}", datasetPath);
            _logger.LogInformation("Output model: {ModelPath}", modelPath);

            // Validate dataset first
            var validation = await _dataLoader.ValidateDatasetAsync(datasetPath);
            
            if (!validation.IsValid)
            {
                _logger.LogError("Dataset validation failed:");
                validation.Errors.ForEach(error => _logger.LogError("  {Error}", error));
                return;
            }

            if (validation.Warnings.Any())
                validation.Warnings.ForEach(warning => _logger.LogWarning("  {Warning}", warning));

            _logger.LogInformation("Found {TotalImages} images across {ClassCount} classes", 
                validation.TotalImages, validation.ClassCounts.Count);
            
            // Show class distribution
            foreach (var (className, count) in validation.ClassCounts.OrderByDescending(x => x.Value))
                _logger.LogInformation("  {Class}: {Count} images", className, count);

            var trainingData = await _dataLoader.LoadImagesFromDirectoryAsync(datasetPath);

            if (trainingData.Length == 0)
            {
                _logger.LogError("No training data found!");
                return;
            }

            _logger.LogInformation("Training ResNetV2101 model (GPU: {UseGpu}, Fallback: {Fallback})...", 
                _modelConfig.UseGpu, _modelConfig.FallbackToCpu);
            var (model, metrics) = await _modelTrainer.TrainModelAsync(trainingData, _modelConfig);

            _logger.LogInformation("Training completed! Time: {TrainingTime:F1}s", metrics.TrainingTimeSeconds);
            _logger.LogInformation("Results: Accuracy {MicroAccuracy:P1}, Loss {LogLoss:F3}", 
                metrics.MicroAccuracy, metrics.LogLoss);

            // Save the model
            var mlContext = new Microsoft.ML.MLContext();
            var schema = mlContext.Data.LoadFromEnumerable<FruitImageData>(new List<FruitImageData>()).Schema;
            
            await _modelTrainer.SaveModelAsync(model, modelPath, schema);
            _logger.LogInformation("Model saved to: {ModelPath}", modelPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Training failed: {Message}", ex.Message);
            throw;
        }
    }

    private string ConvertToAbsolutePath(string path)
    {
        if (Path.IsPathRooted(path))
            return path;

        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
    }
}
