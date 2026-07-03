using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.BackgroundServices;

public class CleanupBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<CleanupBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var scope = scopeFactory.CreateScope();
                var cleanupService = scope.ServiceProvider.GetRequiredService<CleanupService>();
                await cleanupService.CleanupAsync();
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("CleanupBackgroundService stopped");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in CleanupBackgroundService");
            throw;
        }
    }
}