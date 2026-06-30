using System.Net.Http.Json;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace DirectoryService.IntegrationTests;

public abstract class DirectoryServiceTests(
    DirectoryServiceTestWebFactory factory)
    : IClassFixture<DirectoryServiceTestWebFactory>, IAsyncLifetime
{
    protected readonly HttpClient Client = factory.CreateClient();

    protected DirectoryServiceTestWebFactory Factory { get; } = factory;

    private readonly IServiceProvider _services = factory.Services;

    protected async Task<T> ExecuteInDbAsync<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        await using var scope = _services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        return await action(sut);
    }

    protected async Task<Guid> CreateLocationAsync(string? name = null)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var request = new CreateLocationRequest(name ?? $"Location {suffix}",
            new AddressDto
            {
                City = $"City {suffix}",
                Street = $"Street {suffix}",
                Building = 1,
                Postcode = suffix
            },
            "Europe/London");

        using HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        return envelope!.Result;
    }

    protected async Task<Guid> CreateDepartmentAsync(
        IEnumerable<Guid>? locationIds = null,
        string? name = null,
        string? identifier = null)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        Guid[] locations = (locationIds ?? [await CreateLocationAsync()]).ToArray();
        var request = new CreateDepartmentRequest(
            name ?? $"Department {suffix}",
            identifier ?? $"department{LettersOnlySuffix()}",
            null,
            locations);

        using HttpResponseMessage response = await Client.PostAsJsonAsync("api/departments", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        return envelope!.Result;
    }

    private static string LettersOnlySuffix()
    {
        return new string(Guid.NewGuid().ToString("N").Where(char.IsLetter).Take(12).ToArray());
    }

    protected async Task<Guid> CreatePositionAsync(IEnumerable<Guid>? departmentIds = null, string? name = null)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        Guid[] departments = (departmentIds ?? [await CreateDepartmentAsync()]).ToArray();
        var request = new CreatePositionRequest(name ?? $"Position {suffix}", "Test position", departments);

        using HttpResponseMessage response = await Client.PostAsJsonAsync("api/positions", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        return envelope!.Result;
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