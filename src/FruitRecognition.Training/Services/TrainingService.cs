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
        _logger.LogInformation("Starting Fruit Recognition Model Training");

        try
        {
            var datasetPath = args.Length > 0 ? args[0] : _pathConfig.DatasetPath;
            var modelPath = args.Length > 1 ? args[1] : _pathConfig.ModelPath;

            datasetPath = ConvertToAbsolutePath(datasetPath);
            modelPath = ConvertToAbsolutePath(modelPath);

            _logger.LogInformation("Dataset Path: {DatasetPath}", datasetPath);
            _logger.LogInformation("Model Output Path: {ModelPath}", modelPath);

            var validationResult = await _dataLoader.ValidateDatasetAsync(datasetPath);
            
            if (!validationResult.IsValid)
            {
                _logger.LogError("Dataset validation failed:");
                foreach (var error in validationResult.Errors)
                    _logger.LogError("  - {Error}", error);
                return;
            }

            if (validationResult.Warnings.Any())
            {
                foreach (var warning in validationResult.Warnings)
                    _logger.LogWarning("  - {Warning}", warning);
            }

            _logger.LogInformation("Dataset validation successful: {TotalImages} images, {ClassCount} classes", validationResult.TotalImages, validationResult.ClassCounts.Count);
            
            foreach (var classCount in validationResult.ClassCounts.OrderByDescending(x => x.Value))
                _logger.LogInformation("    {Class}: {Count} images", classCount.Key, classCount.Value);

            var trainingData = await _dataLoader.LoadImagesFromDirectoryAsync(datasetPath);

            if (trainingData.Length == 0)
            {
                _logger.LogError("No training data found!");
                return;
            }

            var (model, metrics) = await _modelTrainer.TrainModelAsync(trainingData, _modelConfig);

            _logger.LogInformation("Training completed successfully!");
            _logger.LogInformation("Training Time: {TrainingTime:F2} seconds", metrics.TrainingTimeSeconds);
            _logger.LogInformation("Training Samples: {TrainingSamples}", metrics.TrainingSampleCount);
            _logger.LogInformation("Model Performance Metrics:");
            _logger.LogInformation("  Micro Accuracy: {MicroAccuracy:P2}", metrics.MicroAccuracy);
            _logger.LogInformation("  Macro Accuracy: {MacroAccuracy:P2}", metrics.MacroAccuracy);
            _logger.LogInformation("  Log Loss: {LogLoss:F4}", metrics.LogLoss);

            var mlContext = new Microsoft.ML.MLContext();
            var schema = mlContext.Data.LoadFromEnumerable<FruitImageData>(new List<FruitImageData>()).Schema;
            
            await _modelTrainer.SaveModelAsync(model, modelPath, schema);
            
            _logger.LogInformation("Model saved successfully to: {ModelPath}", modelPath);
            _logger.LogInformation("Training completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Training failed with error: {Message}", ex.Message);
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
