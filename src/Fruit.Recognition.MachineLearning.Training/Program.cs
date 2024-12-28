using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;

namespace FruitIdentifier
{
    class Program
    {
        public class ImageData
        {
            public string ImagePath { get; set; }
            public string Label { get; set; }
        }

        static void Main(string[] args)
        {
            var mlContext = new MLContext();

            string datasetPath = "C:\\Users\\Gomes\\Desktop\\test\\dataset\\training\\fruits";

            Console.WriteLine("Loading dataset from: " + datasetPath);

            var imageData = LoadImagesFromDirectory(datasetPath);
            var data = mlContext.Data.LoadFromEnumerable(imageData);

            Console.WriteLine($"Loaded {imageData.Length} images across {Directory.GetDirectories(datasetPath).Length} classes.");

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(ImageData.Label))
                .Append(mlContext.Transforms.LoadImages(
                    outputColumnName: "Image",
                    imageFolder: datasetPath,
                    inputColumnName: nameof(ImageData.ImagePath)))
                .Append(mlContext.Transforms.ResizeImages(
                    outputColumnName: "Image",
                    imageWidth: 224,
                    imageHeight: 224))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "Image"))
                .Append(mlContext.Transforms.Concatenate("Features", "Image"))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));

            Console.WriteLine("Training the model...");
            var trainedModel = pipeline.Fit(data);

            Console.WriteLine("Saving the model...");
            var finalPipeline = pipeline.Append(mlContext.Transforms.DropColumns("Label"));
            var modelPath = "C:\\Users\\Gomes\\Desktop\\test\\dataset\\test\\fruitModel.zip";
            mlContext.Model.Save(finalPipeline.Fit(data), data.Schema, modelPath);

            Console.WriteLine($"Model saved successfully to {modelPath}");
        }

        private static ImageData[] LoadImagesFromDirectory(string folder)
        {
            var directories = Directory.GetDirectories(folder);
            var imageList = new List<ImageData>();

            foreach (var directory in directories)
            {
                var label = Path.GetFileName(directory);
                var files = Directory.GetFiles(directory, "*.jpg");

                foreach (var file in files)
                {
                    imageList.Add(new ImageData
                    {
                        ImagePath = file,
                        Label = label
                    });
                }
            }

            return [.. imageList];
        }
    }
}
