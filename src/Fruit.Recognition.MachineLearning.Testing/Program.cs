using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Fruit.Recognition.MachineLearning.Domain.Models;

namespace Fruit.Recognition.MachineLearning.Testing
{
    class Program
    {
        public class ImagePrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabel { get; set; }
        }

        static void Main(string[] args)
        {
            // Configuração inicial do MLContext
            var mlContext = new MLContext();

            // Caminho do modelo salvo
            string modelPath = "C:\\Users\\Gomes\\Desktop\\test\\dataset\\test\\fruitModel.zip";

            if (!File.Exists(modelPath))
            {
                Console.WriteLine($"Error: Model file not found at {modelPath}");
                return;
            }

            Console.WriteLine("Loading the model...");
            var loadedModel = mlContext.Model.Load(modelPath, out var inputSchema);

            // Inspeciona o esquema do modelo carregado
            Console.WriteLine("Inspecting input schema:");
            foreach (var column in inputSchema)
            {
                Console.WriteLine($"  Column: {column.Name}, Type: {column.Type}");
            }

            // Cria o PredictionEngine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(loadedModel);

            // Caminho da imagem a ser testada
            string imagePath = "C:\\Users\\Gomes\\Desktop\\test\\dataset\\test\\fruit.jpg";

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Error: Image file not found at {imagePath}");
                return;
            }

            // Cria o objeto de entrada para predição
            var sampleImage = new ImageData { ImagePath = imagePath };

            Console.WriteLine($"Making prediction for image: {imagePath}");

            // Debug: Transformação de dados
            var inputData = mlContext.Data.LoadFromEnumerable(new[] { sampleImage });
            var transformedData = loadedModel.Transform(inputData);

            Console.WriteLine("Preview of transformed data:");
            var preview = transformedData.Preview();

            foreach (var row in preview.RowView)
            {
                foreach (var column in row.Values)
                {
                    Console.WriteLine($"  {column.Key}: {column.Value}");
                }
            }

            // Realiza a predição
            var prediction = predictionEngine.Predict(sampleImage);

            Console.WriteLine($"Predicted fruit: {prediction.PredictedLabel}");

            // Testa uma imagem do treinamento (opcional, para debug)
            string trainingImagePath = "C:\\Users\\Gomes\\Desktop\\test\\dataset\\test\\fruit3.jpg";
            if (File.Exists(trainingImagePath))
            {
                Console.WriteLine($"Testing with a training image: {trainingImagePath}");
                var trainingSample = new ImageData { ImagePath = trainingImagePath };
                var trainingPrediction = predictionEngine.Predict(trainingSample);
                Console.WriteLine($"Predicted label for training image: {trainingPrediction.PredictedLabel}");
            }

            // Verifica métricas do modelo, se necessário
            Console.WriteLine("Additional checks completed.");
        }
    }
}
