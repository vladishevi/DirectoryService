using System.Net;
using System.Net.Http.Json;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared;
using Shared.Errors;

namespace DirectoryService.IntegrationTests.Locations;

public class GetTopLocationsTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task GetTopLocations_should_succeed()
    {
        await CreateLocationAsync("Top Location");

        HttpResponseMessage response = await Client.GetAsync("api/locations/top");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<List<LocationTopDto>>>();
        
        var ids = envelope.Result.Select(l => l.Id);
        var locations = await ExecuteInDbAsync(async dbContext =>
            await dbContext
                .LocationsRead
                .Where(l => ids.Contains(l.Id))
                .ToListAsync());

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.NotNull(envelope.Result);
        Assert.True(envelope.Result.Count <= 5);
        Assert.All(envelope.Result, location => Assert.NotNull(location.Address));
        Assert.All(locations, location => Assert.False(location.IsDeleted));
    }

    [Fact]
    public async Task GetTopLocations_when_database_fails_should_fail()
    {
        using var webFactory = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IDbConnectionFactory>();
                services.AddSingleton<IDbConnectionFactory, BrokenDbConnectionFactory>();
            });
        });
        using HttpClient client = webFactory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("api/locations/top");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.FAILURE);
    }
}