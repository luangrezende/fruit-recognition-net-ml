using FruitRecognition.Core.Models;

namespace FruitRecognition.Core.Services;

public interface IDataLoaderService
{
    Task<FruitImageData[]> LoadImagesFromDirectoryAsync(string datasetPath);
    Task<DatasetValidationResult> ValidateDatasetAsync(string datasetPath);
}

public class DatasetValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, int> ClassCounts { get; set; } = new();
    public int TotalImages { get; set; }
}
