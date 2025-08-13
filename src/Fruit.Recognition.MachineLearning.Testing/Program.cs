using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using FruitRecognition.Core.Services;
using FruitRecognition.Core.Configuration;
using FruitRecognition.Prediction.Services;

namespace FruitRecognition.Prediction;

class Program
{
    static async Task Main(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddCommandLine(args)
            .Build();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.Configure<PathConfiguration>(configuration.GetSection(nameof(PathConfiguration)));
                services.AddSingleton<IPredictionService, PredictionService>();
                services.AddSingleton<PredictionApplicationService>();
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
            var predictionApp = host.Services.GetRequiredService<PredictionApplicationService>();
            await predictionApp.RunAsync(args);
        }
        catch (Exception ex)
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Prediction failed: {Message}", ex.Message);
            Environment.ExitCode = 1;
        }
        finally
        {
            Console.WriteLine("\n=== Prediction Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            await host.StopAsync();
        }
    }
}
