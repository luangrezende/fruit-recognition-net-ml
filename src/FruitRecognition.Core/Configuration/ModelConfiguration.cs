namespace FruitRecognition.Core.Configuration;

public class ModelConfiguration
{
    public int ImageWidth { get; set; } = 224;
    public int ImageHeight { get; set; } = 224;
    public double TestFraction { get; set; } = 0.2;
    public int? Seed { get; set; } = 42;
    public int? ProcessorCount { get; set; }
    public int Epochs { get; set; } = 200;
    public int BatchSize { get; set; } = 10;
    public float LearningRate { get; set; } = 0.01f;
    public string Architecture { get; set; } = "ResNetV2101";
    public bool UseTransferLearning { get; set; } = true;
    public double ValidationFraction { get; set; } = 0.1;
}

public class PathConfiguration
{
    public string DatasetPath { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
    public string TestImagesPath { get; set; } = string.Empty;
}
