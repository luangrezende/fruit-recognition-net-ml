using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace FruitPrediction
{
    class Program
    {
        public class ImageData
        {
            public string ImagePath { get; set; }
        }

        public class ImagePrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabel { get; set; }
        }

        static void Main(string[] args)
        {
            var mlContext = new MLContext();

            string modelPath = "C:\\Users\\Gomes\\Desktop\\test\\dataset\\test\\fruitModel.zip";

            if (!File.Exists(modelPath))
            {
                Console.WriteLine($"Error: Model file not found at {modelPath}");
                return;
            }

            Console.WriteLine("Loading the model...");
            var loadedModel = mlContext.Model.Load(modelPath, out var inputSchema);

            Console.WriteLine("Inspecting input schema:");
            foreach (var column in inputSchema)
            {
                Console.WriteLine($"  Column: {column.Name}, Type: {column.Type}");
            }

            var predictionEngine = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(loadedModel);

            string imagePath = "C:\\Users\\Gomes\\Desktop\\test\\dataset\\test\\fruit.jpg";

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Error: Image file not found at {imagePath}");
                return;
            }

            var sampleImage = new ImageData { ImagePath = imagePath };

            Console.WriteLine($"Making prediction for image: {imagePath}");
            var prediction = predictionEngine.Predict(sampleImage);

            Console.WriteLine($"Predicted fruit: {prediction.PredictedLabel}");
        }
    }
}
