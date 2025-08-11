using Microsoft.ML;
using FruitRecognition.Core.Models;
using FruitRecognition.Core.Configuration;

namespace FruitRecognition.Core.Services;

public interface IModelTrainerService
{
    Task<(ITransformer Model, ModelMetrics Metrics)> TrainModelAsync(FruitImageData[] trainingData, ModelConfiguration config);
    Task<ModelMetrics> EvaluateModelAsync(ITransformer model, IDataView testData, MLContext mlContext);
    Task SaveModelAsync(ITransformer model, string modelPath, DataViewSchema schema);
}
