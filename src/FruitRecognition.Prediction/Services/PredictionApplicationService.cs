using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using FruitRecognition.Core.Services;
using FruitRecognition.Core.Configuration;
using FruitRecognition.Core.Models;

namespace FruitRecognition.Prediction.Services;

public class PredictionApplicationService
{
    private readonly ILogger<PredictionApplicationService> _logger;
    private readonly IPredictionService _predictionService;
    private readonly PathConfiguration _pathConfig;

    public PredictionApplicationService(
        ILogger<PredictionApplicationService> logger,
        IPredictionService predictionService,
        IOptions<PathConfiguration> pathConfig)
    {
        _logger = logger;
        _predictionService = predictionService;
        _pathConfig = pathConfig.Value;
    }

    public async Task RunAsync(string[] args)
    {
        try
        {
            var modelPath = args.Length > 0 ? args[0] : _pathConfig.ModelPath;
            var imagePath = args.Length > 1 ? args[1] : null;

            modelPath = ConvertToAbsolutePath(modelPath);

            var (model, schema) = await _predictionService.LoadModelAsync(modelPath);
            
            var mlContext = new MLContext();
            var predictionEngine = _predictionService.CreatePredictionEngine(model, mlContext);

            if (!string.IsNullOrEmpty(imagePath))
            {
                await PredictSingleImageAsync(imagePath, predictionEngine);
            }
            else
            {
                // Run batch mode automatically for all images in identification folder
                _logger.LogInformation("Processing all images in identification folder...");
                await RunBatchModeAsync(predictionEngine);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Prediction failed with error: {Message}", ex.Message);
            throw;
        }
    }

    private async Task PredictSingleImageAsync(
        string imagePath, 
        PredictionEngine<FruitImageData, FruitPrediction> predictionEngine)
    {
        imagePath = ConvertToAbsolutePath(imagePath);

        var prediction = await _predictionService.PredictAsync(imagePath, predictionEngine);
        
        _logger.LogInformation("Result: {PredictedLabel} (confidence: {Confidence:F1}%)", 
            prediction.PredictedLabel, prediction.Confidence);
        
        // Show top predictions if we have multiple classes
        if (prediction.Score?.Length > 1)
        {
            var topScores = prediction.Score
                .Select((score, index) => new { Index = index, Score = score })
                .OrderByDescending(x => x.Score)
                .Take(3); // Show top 3 instead of 5

            foreach (var item in topScores)
            {
                _logger.LogInformation("  Class {Index}: {Score:P1}", item.Index, item.Score);
            }
        }
    }

    private async Task RunInteractiveModeAsync(
        PredictionEngine<FruitImageData, FruitPrediction> predictionEngine)
    {
        _logger.LogInformation("Interactive mode - Enter image paths to predict ('quit' to exit):");

        while (true)
        {
            Console.Write("Image path > ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                break;

            if (input.Equals("batch", StringComparison.OrdinalIgnoreCase))
            {
                await RunBatchModeAsync(predictionEngine);
                continue;
            }

            try
            {
                await PredictSingleImageAsync(input, predictionEngine);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to predict image '{ImagePath}': {Message}", input, ex.Message);
            }
        }
    }

    private async Task RunBatchModeAsync(
        PredictionEngine<FruitImageData, FruitPrediction> predictionEngine)
    {
        var testImagesPath = ConvertToAbsolutePath(_pathConfig.TestImagesPath ?? Path.Combine(Directory.GetCurrentDirectory(), "data", "identification"));

        if (!Directory.Exists(testImagesPath))
        {
            _logger.LogWarning("Identification images directory not found: {TestImagesPath}", testImagesPath);
            _logger.LogInformation("Please add images to the identification folder to process them.");
            return;
        }

        var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
        var imageFiles = new List<string>();

        foreach (var extension in supportedExtensions)
        {
            imageFiles.AddRange(Directory.GetFiles(testImagesPath, $"*{extension}", SearchOption.AllDirectories));
        }

        if (imageFiles.Count == 0)
        {
            _logger.LogWarning("No supported image files found in the test directory");
            return;
        }

        var results = await _predictionService.PredictBatchAsync(imageFiles, predictionEngine);

        foreach (var (imagePath, prediction) in results)
        {
            var fileName = Path.GetFileName(imagePath);
            _logger.LogInformation("{FileName} -> {PredictedLabel} ({Confidence:F1}%)", 
                fileName, prediction.PredictedLabel, prediction.Confidence);
        }

        var predictions = results.Select(r => r.Prediction).ToList();
        var avgConfidence = predictions.Average(p => p.Confidence);
        var predictionCounts = predictions.GroupBy(p => p.PredictedLabel)
            .ToDictionary(g => g.Key, g => g.Count());

        _logger.LogInformation("Total processed: {Count}", results.Count);
        _logger.LogInformation("Average confidence: {AvgConfidence:F2}%", avgConfidence);
        
        foreach (var kvp in predictionCounts.OrderByDescending(x => x.Value))
        {
            _logger.LogInformation("{Class}: {Count} images", kvp.Key, kvp.Value);
        }
    }

    private string ConvertToAbsolutePath(string path)
    {
        if (Path.IsPathRooted(path))
            return path;

        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
    }
}
