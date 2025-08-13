using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using FruitRecognition.Core.Services;
using FruitRecognition.Core.Configuration;
using FruitRecognition.Training.Services;

namespace FruitRecognition.Training;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddCommandLine(args)
            .Build();

        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.Configure<ModelConfiguration>(config.GetSection(nameof(ModelConfiguration)));
                services.Configure<PathConfiguration>(config.GetSection(nameof(PathConfiguration)));
                
                // Register our services
                services.AddSingleton<IDataLoaderService, DataLoaderService>();
                services.AddSingleton<IModelTrainerService, ModelTrainerService>();
                services.AddSingleton<TrainingService>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        try
        {
            var trainingService = host.Services.GetRequiredService<TrainingService>();
            await trainingService.RunAsync(args);
        }
        catch (Exception ex)
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Training failed: {Message}", ex.Message);
            Environment.ExitCode = 1;
        }
        finally
        {
            Console.WriteLine("\n=== Training Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
