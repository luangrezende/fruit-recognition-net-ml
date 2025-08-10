using Microsoft.ML;
using FruitRecognition.Core.Models;

namespace FruitRecognition.Core.Services;

/// <summary>
/// Interface for making predictions with trained models.
/// </summary>
public interface IPredictionService
{
    /// <summary>
    /// Loads a trained model from the specified path.
    /// </summary>
    /// <param name="modelPath">Path to the model file</param>
    /// <returns>Loaded model and input schema</returns>
    Task<(ITransformer Model, DataViewSchema Schema)> LoadModelAsync(string modelPath);

    /// <summary>
    /// Creates a prediction engine for the loaded model.
    /// </summary>
    /// <param name="model">Trained model</param>
    /// <param name="mlContext">ML context</param>
    /// <returns>Prediction engine</returns>
    PredictionEngine<FruitImageData, FruitPrediction> CreatePredictionEngine(
        ITransformer model, 
        MLContext mlContext);

    /// <summary>
    /// Predicts the fruit type for a single image.
    /// </summary>
    /// <param name="imagePath">Path to the image</param>
    /// <param name="predictionEngine">Prediction engine</param>
    /// <returns>Prediction result</returns>
    Task<FruitPrediction> PredictAsync(
        string imagePath, 
        PredictionEngine<FruitImageData, FruitPrediction> predictionEngine);

    /// <summary>
    /// Predicts the fruit type for multiple images.
    /// </summary>
    /// <param name="imagePaths">Paths to the images</param>
    /// <param name="predictionEngine">Prediction engine</param>
    /// <returns>Prediction results</returns>
    Task<List<(string ImagePath, FruitPrediction Prediction)>> PredictBatchAsync(
        IEnumerable<string> imagePaths,
        PredictionEngine<FruitImageData, FruitPrediction> predictionEngine);
}
