using DirectoryService.IntegrationTests.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure.Postgres.BackgroundServices;

public class CleanupService(
    DirectoryServiceDbContext dbContext,
    IOptions<CleanupOptions> options,
    ILogger<CleanupService> logger)
{
    public async Task CleanupAsync(CancellationToken stoppingToken = default)
    {
        logger.LogInformation("Cleaning up database");

        var cutoff = DateTime.UtcNow - options.Value.RetentionPeriod;
        int totalDeleted = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            int deleted = await dbContext.Positions
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted && p.DeletedAt < cutoff)
                .Take(options.Value.BatchSize)
                .ExecuteDeleteAsync(stoppingToken);

            totalDeleted += deleted;
            
            if (deleted < options.Value.BatchSize)
                break;
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            int deleted = await dbContext.Locations
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted && p.DeletedAt < cutoff)
                .Take(options.Value.BatchSize)
                .ExecuteDeleteAsync(stoppingToken);

            totalDeleted += deleted;
            
            if (deleted < options.Value.BatchSize)
                break;
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            int deleted = await dbContext.DepartmentsRead
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted && p.DeletedAt < cutoff)
                .Take(options.Value.BatchSize)
                .ExecuteDeleteAsync(stoppingToken);

            totalDeleted += deleted;
            
            if (deleted < options.Value.BatchSize)
                break;
        }

        logger.LogInformation("{deleted} rows have been deleted", totalDeleted);
    } 
}
