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
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddCommandLine(args)
            .Build();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
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
            var predictionService = host.Services.GetRequiredService<PredictionApplicationService>();
            await predictionService.RunAsync(args);
        }
        catch (Exception ex)
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during prediction");
            Environment.ExitCode = 1;
        }
        finally
        {
            await host.StopAsync();
        }
    }
}
