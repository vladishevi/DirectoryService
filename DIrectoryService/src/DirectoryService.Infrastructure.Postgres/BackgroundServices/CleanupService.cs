using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.BackgroundServices;

public class CleanupService(
    DirectoryServiceDbContext dbContext, 
    ILogger<CleanupService> logger)
{
    public async Task CleanupAsync()
    {
        logger.LogInformation("Cleaning up database");
        
    } 
}