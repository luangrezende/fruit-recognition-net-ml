using Microsoft.ML.Data;

namespace FruitRecognition.Core.Models;

public class FruitImageData
{
    [ColumnName("ImagePath")]
    public string ImagePath { get; set; } = string.Empty;

    [ColumnName("Label")]
    public string Label { get; set; } = string.Empty;
}
