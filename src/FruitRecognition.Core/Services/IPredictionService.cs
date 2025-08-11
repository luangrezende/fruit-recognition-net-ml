using Microsoft.ML;
using FruitRecognition.Core.Models;

namespace FruitRecognition.Core.Services;

public interface IPredictionService
{
    Task<(ITransformer Model, DataViewSchema Schema)> LoadModelAsync(string modelPath);

    PredictionEngine<FruitImageData, FruitPrediction> CreatePredictionEngine(
        ITransformer model, 
        MLContext mlContext);

    Task<FruitPrediction> PredictAsync(
        string imagePath, 
        PredictionEngine<FruitImageData, FruitPrediction> predictionEngine);

    Task<List<(string ImagePath, FruitPrediction Prediction)>> PredictBatchAsync(
        IEnumerable<string> imagePaths,
        PredictionEngine<FruitImageData, FruitPrediction> predictionEngine);
}
