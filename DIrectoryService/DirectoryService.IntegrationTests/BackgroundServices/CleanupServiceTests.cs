using DirectoryService.Infrastructure.Postgres.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.BackgroundServices;

public class CleanupServiceTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task Cleanup_should_succeed()
    {
        //ARRANGE
        //create location
        var locationId = await CreateLocationAsync("Cleanup location");

        //mark location as deleted
        await Client.DeleteAsync($"api/locations/{locationId}");
        await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.Locations
                .IgnoreQueryFilters()
                .Where(l => l.Id == locationId)
                .ExecuteUpdateAsync(setters =>
                {
                    setters.SetProperty(l => l.DeletedAt, DateTime.UtcNow.AddDays(-7));
                });
        });
        
        //create service
        await using var scope = factory.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<CleanupService>();

        //ACT
        await service.CleanupAsync();
        var location = await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext
                .Locations
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(l => l.Id == locationId);
        });

        //ASSERT
        Assert.Null(location);
    }    
}