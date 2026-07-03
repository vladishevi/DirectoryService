using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.BackgroundServices;

public class CleanupService(
    DirectoryServiceDbContext dbContext, 
    ILogger<CleanupService> logger)
{
    public async Task CleanupAsync()
    {
        logger.LogInformation("Cleaning up database");
        
        int deleted = await dbContext.Locations.
            IgnoreQueryFilters()
            .Where(l => l.IsDeleted && DateTime.UtcNow >= l.DeletedAt.AddMinutes(1))
            .ExecuteDeleteAsync();
        
        logger.LogInformation("{deleted} rows have been deleted", deleted);
        
    } 
}