using DirectoryService.Infrastructure.Postgres.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DirectoryService.IntegrationTests.BackgroundServices;

public class CleanupServiceTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task Cleanup_expired_location_should_succeed()
    {
        //ARRANGE
        await using var scope = factory.Services.CreateAsyncScope();
        
        //create location
        var locationId = await CreateLocationAsync("Cleanup location");
        var cleanupOptions = scope.ServiceProvider.GetRequiredService<IOptions<CleanupOptions>>();

        //mark location as deleted
        await Client.DeleteAsync($"api/locations/{locationId}");
        await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.Locations
                .IgnoreQueryFilters()
                .Where(l => l.Id == locationId)
                .ExecuteUpdateAsync(setters =>
                {
                    DateTime expiredDate = DateTime.UtcNow.Add(-cleanupOptions.Value.RetentionPeriod);
                    setters.SetProperty(l => l.DeletedAt, expiredDate);
                });
        });
        
        //create service
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
    
    [Fact]
    public async Task Cleanup_unexpired_location_should_fail()
    {
        //ARRANGE
        //create location
        var locationId = await CreateLocationAsync("Cleanup location");

        //mark location as deleted
        await Client.DeleteAsync($"api/locations/{locationId}");
        
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
        Assert.NotNull(location);
    }    
}