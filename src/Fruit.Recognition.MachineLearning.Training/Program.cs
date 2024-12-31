using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Fruit.Recognition.MachineLearning.Domain.Models;

namespace Fruit.Recognition.MachineLearning.Training
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configurações iniciais
            var mlContext = new MLContext(Environment.ProcessorCount);

            // Caminho do dataset
            string datasetPath = args.Length > 0 ? args[0] : "C:\\Users\\Gomes\\Desktop\\test\\dataset\\training\\fruits";

            if (!Directory.Exists(datasetPath))
            {
                Console.WriteLine($"Error: Dataset path '{datasetPath}' does not exist.");
                return;
            }

            Console.WriteLine("Loading dataset from: " + datasetPath);

            // Carrega imagens e valida o dataset
            var imageData = LoadImagesFromDirectory(datasetPath);
            if (imageData.Length == 0)
            {
                Console.WriteLine("Error: No images found in the dataset directory.");
                return;
            }

            Console.WriteLine($"Loaded {imageData.Length} images across {Directory.GetDirectories(datasetPath).Length} classes.");

            foreach (var dir in Directory.GetDirectories(datasetPath))
            {
                Console.WriteLine($" - {Path.GetFileName(dir)}: {Directory.GetFiles(dir, "*.jpg").Length} images");
            }

            // Converte os dados para IDataView
            var data = mlContext.Data.LoadFromEnumerable(imageData);

            // Divisão do dataset em treinamento e validação
            var trainTestData = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            var trainData = trainTestData.TrainSet;
            var testData = trainTestData.TestSet;

            // Define o pipeline de treinamento
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
             .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                 labelColumnName: "Label",
                 featureColumnName: "Features"))
             .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Label"));


            // Treina o modelo com temporizador
            Console.WriteLine("Training the model...");
            var stopwatch = Stopwatch.StartNew();
            var trainedModel = pipeline.Fit(trainData);
            stopwatch.Stop();
            Console.WriteLine($"Model training completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");

            // Avalia o modelo com o conjunto de validação
            Console.WriteLine("Evaluating the model...");
            var metrics = mlContext.MulticlassClassification.Evaluate(trainedModel.Transform(testData), "Label");

            Console.WriteLine($"Metrics:");
            Console.WriteLine($"  MicroAccuracy: {metrics.MicroAccuracy:F2}");
            Console.WriteLine($"  MacroAccuracy: {metrics.MacroAccuracy:F2}");
            Console.WriteLine($"  LogLoss: {metrics.LogLoss:F2}");

            // Exibe a matriz de confusão
            Console.WriteLine("Confusion Matrix:");
            for (int i = 0; i < metrics.ConfusionMatrix.NumberOfClasses; i++)
            {
                Console.WriteLine($"Class {i}: {string.Join(", ", metrics.ConfusionMatrix.Counts[i])}");
            }

            // Salva o modelo treinado
            Console.WriteLine("Saving the model...");
            var modelPath = "C:\\Users\\Gomes\\Desktop\\test\\dataset\\test\\fruitModel.zip";
            mlContext.Model.Save(trainedModel, trainData.Schema, modelPath);

            Console.WriteLine($"Model saved successfully to {modelPath}");
        }

        /// <summary>
        /// Carrega imagens de um diretório e associa rótulos com base no nome do subdiretório.
        /// </summary>
        private static ImageData[] LoadImagesFromDirectory(string folder)
        {
            var directories = Directory.GetDirectories(folder);
            var imageList = new List<ImageData>();

            foreach (var directory in directories)
            {
                var label = Path.GetFileName(directory); // Nome do diretório é o rótulo
                var files = Directory.GetFiles(directory, "*.jpg");

                if (files.Length == 0)
                {
                    Console.WriteLine($"Warning: No images found for label '{label}'.");
                    continue;
                }

                foreach (var file in files)
                {
                    imageList.Add(new ImageData
                    {
                        ImagePath = file,
                        Label = label
                    });
                }
            }

            return imageList.ToArray();
        }
    }

    /// <summary>
    /// Classe que representa uma imagem e seu rótulo.
    /// </summary>
    public class ImageData
    {
        public string ImagePath { get; set; }
        public string Label { get; set; }
    }
}
