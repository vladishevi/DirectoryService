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
    public async Task CleanupAsync()
    {
        logger.LogInformation("Cleaning up database");

        var cutoff = DateTime.UtcNow - options.Value.RetentionPeriod;
        int deleted = await dbContext.Locations.
            IgnoreQueryFilters()
            .Where(l => l.IsDeleted && l.DeletedAt < cutoff)
            .ExecuteDeleteAsync();
        
        logger.LogInformation("{deleted} rows have been deleted", deleted);
        
    } 
}