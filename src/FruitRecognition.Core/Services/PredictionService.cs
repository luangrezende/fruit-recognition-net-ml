using Microsoft.Extensions.Logging;
using Microsoft.ML;
using FruitRecognition.Core.Models;

namespace FruitRecognition.Core.Services;

public class PredictionService : IPredictionService
{
    private readonly ILogger<PredictionService> _logger;

    public PredictionService(ILogger<PredictionService> logger)
    {
        _logger = logger;
    }

    public async Task<(ITransformer Model, DataViewSchema Schema)> LoadModelAsync(string modelPath)
    {
        _logger.LogInformation("Loading model from: {ModelPath}", modelPath);

        if (!File.Exists(modelPath))
            throw new FileNotFoundException($"Model file not found: {modelPath}");

        var mlContext = new MLContext();
        var loadedModel = mlContext.Model.Load(modelPath, out var inputSchema);

        _logger.LogInformation("Model loaded successfully");
        return await Task.FromResult((loadedModel, inputSchema));
    }

    public PredictionEngine<FruitImageData, FruitPrediction> CreatePredictionEngine(ITransformer model, MLContext mlContext)
    {
        return mlContext.Model.CreatePredictionEngine<FruitImageData, FruitPrediction>(model);
    }

    public async Task<FruitPrediction> PredictAsync(string imagePath, PredictionEngine<FruitImageData, FruitPrediction> predictionEngine)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException($"Image file not found: {imagePath}");

        var imageData = new FruitImageData { ImagePath = imagePath };
        var prediction = predictionEngine.Predict(imageData);

        return await Task.FromResult(prediction);
    }

    public async Task<List<(string ImagePath, FruitPrediction Prediction)>> PredictBatchAsync(IEnumerable<string> imagePaths, PredictionEngine<FruitImageData, FruitPrediction> predictionEngine)
    {
        var results = new List<(string ImagePath, FruitPrediction Prediction)>();
        
        foreach (var imagePath in imagePaths)
        {
            try
            {
                var prediction = await PredictAsync(imagePath, predictionEngine);
                results.Add((imagePath, prediction));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to predict for image: {ImagePath}", imagePath);
            }
        }

        return results;
    }
}
