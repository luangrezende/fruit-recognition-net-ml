using Microsoft.ML.Data;

namespace FruitRecognition.Core.Models;

public class FruitPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; } = string.Empty;

    [ColumnName("Score")]
    public float[] Score { get; set; } = Array.Empty<float>();

    public double Confidence => Score?.Max() * 100 ?? 0;
}
