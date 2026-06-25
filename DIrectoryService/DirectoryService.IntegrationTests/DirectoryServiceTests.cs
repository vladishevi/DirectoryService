using DirectoryService.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests;

public abstract class DirectoryServiceTests(
    DirectoryServiceTestWebFactory factory)
    : IClassFixture<DirectoryServiceTestWebFactory>, IAsyncLifetime
{
    protected readonly HttpClient Client = factory.CreateClient();
    
    private readonly IServiceProvider _services = factory.Services;

    protected async Task<T> ExecuteInDbAsync<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        await using var scope = _services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        return await action(sut);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await factory.ResetDbAsync();       
    }
}