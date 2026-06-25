using System.Net.Http.Json;
using DirectoryService.Contracts.Locations;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace DirectoryService.IntegrationTests.Locations;

public class CreateLocationTests : IClassFixture<DirectoryServiceTestWebFactory>, IAsyncLifetime
{
    private readonly IServiceProvider _services;
    private readonly DirectoryServiceTestWebFactory _webFactory;
    private readonly HttpClient _client;

    public CreateLocationTests(DirectoryServiceTestWebFactory factory)
    {
        _services = factory.Services;
        _webFactory = factory;
        _client = factory.CreateClient();       
    }
    
    [Fact]
    public async void CreateLocation_with_valid_data_should_succeed()
    {
        //arrange
        var ct = CancellationToken.None;
        var request = new CreateLocationRequest("My locfatgion test",
            new AddressDto { City = "my city", Street = "my strgeet", Building = 1, Postcode = "92" }, "Europe/London");
        
        //act
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/locations", request, ct);
        Envelope<Guid> envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>(ct);

        
        var location = await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.LocationsRead.FirstOrDefaultAsync(l => l.Id == envelope.Result, ct);
        });
        

        //assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(envelope.Errors);
        Assert.NotNull(envelope.Result);
        Assert.True(envelope.IsSuccess);
        Assert.NotEqual(Guid.Empty, envelope.Result);
        Assert.NotNull(location);
    }
    
    [Fact]
    public async void CreateLocation_with_invalid_timezone_should_fail()
    {
        //arrange
        var ct = CancellationToken.None;
        var request = new CreateLocationRequest("My locfatgion test",
            new AddressDto { City = "my city", Street = "my strgeet", Building = 1, Postcode = "92" }, "");
        
        //act
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/locations", request, ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>(ct);
        bool anyLocationExists = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.AnyAsync(cancellationToken: ct));

        //assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(envelope.Errors);
        Assert.False(envelope.IsSuccess);
        Assert.False(anyLocationExists);
    }

    private async Task<T> ExecuteInDbAsync<T>(Func<DirectoryServiceDbContext, Task<T>> action)
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
        await _webFactory.ResetDbAsync();       
    }
}