using Microsoft.ML;
using Microsoft.ML.Data;

namespace FruitRecognition.Core.Models;

public class ModelMetrics
{
    public double MicroAccuracy { get; set; }
    public double MacroAccuracy { get; set; }
    public double LogLoss { get; set; }
    public ConfusionMatrix? ConfusionMatrix { get; set; }
    public double TrainingTimeSeconds { get; set; }
    public int NumberOfClasses { get; set; }
    public int TrainingSampleCount { get; set; }
    public int TestSampleCount { get; set; }
}
