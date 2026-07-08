using DirectoryService.IntegrationTests.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure.Postgres.BackgroundServices;

public class CleanupBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<CleanupOptions> options,
    ILogger<CleanupBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var timer = new PeriodicTimer(options.Value.Interval);

            do
            {
                try
                {
                    var scope = scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<CleanupService>();
                    await service.CleanupAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "An error occurred while cleaning up old records");
                }
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}