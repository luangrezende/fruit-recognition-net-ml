using Microsoft.Extensions.Logging;
using FruitRecognition.Core.Models;

namespace FruitRecognition.Core.Services;

public class DataLoaderService : IDataLoaderService
{
    private readonly ILogger<DataLoaderService> _logger;
    private readonly string[] _supportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };

    public DataLoaderService(ILogger<DataLoaderService> logger)
    {
        _logger = logger;
    }

    public async Task<FruitImageData[]> LoadImagesFromDirectoryAsync(string datasetPath)
    {
        _logger.LogInformation("Loading images from directory: {DatasetPath}", datasetPath);

        if (!Directory.Exists(datasetPath))
            throw new DirectoryNotFoundException($"Dataset directory not found: {datasetPath}");

        var imageList = new List<FruitImageData>();
        var directories = Directory.GetDirectories(datasetPath);

        foreach (var directory in directories)
        {
            var label = Path.GetFileName(directory);
            var files = GetImageFiles(directory);

            if (files.Length == 0)
            {
                _logger.LogWarning("No supported images found for label '{Label}' in directory '{Directory}'", label, directory);
                continue;
            }

            foreach (var file in files)
            {
                imageList.Add(new FruitImageData { ImagePath = file, Label = label });
            }

            _logger.LogInformation("Loaded {Count} images for label '{Label}'", files.Length, label);
        }

        _logger.LogInformation("Total images loaded: {Count} across {Classes} classes", imageList.Count, directories.Length);
        return await Task.FromResult(imageList.ToArray());
    }

    public async Task<DatasetValidationResult> ValidateDatasetAsync(string datasetPath)
    {
        var result = new DatasetValidationResult();

        if (!Directory.Exists(datasetPath))
        {
            result.Errors.Add($"Dataset directory not found: {datasetPath}");
            return await Task.FromResult(result);
        }

        var directories = Directory.GetDirectories(datasetPath);
        if (directories.Length == 0)
        {
            result.Errors.Add("No subdirectories found. Each fruit category should be in its own subdirectory.");
            return await Task.FromResult(result);
        }

        if (directories.Length < 2)
            result.Warnings.Add("Only one class found. Multi-class classification requires at least 2 classes.");

        foreach (var directory in directories)
        {
            var label = Path.GetFileName(directory);
            var files = GetImageFiles(directory);
            
            result.ClassCounts[label] = files.Length;
            result.TotalImages += files.Length;

            if (files.Length == 0)
                result.Warnings.Add($"No images found for class '{label}'");
            else if (files.Length < 10)
                result.Warnings.Add($"Very few images ({files.Length}) found for class '{label}'. Consider adding more for better training.");
        }

        if (result.TotalImages == 0)
            result.Errors.Add("No valid images found in the dataset.");

        result.IsValid = result.Errors.Count == 0;
        return await Task.FromResult(result);
    }

    private string[] GetImageFiles(string directory)
    {
        var allFiles = new List<string>();
        foreach (var extension in _supportedExtensions)
        {
            allFiles.AddRange(Directory.GetFiles(directory, $"*{extension}", SearchOption.TopDirectoryOnly));
        }
        return allFiles.ToArray();
    }
}
