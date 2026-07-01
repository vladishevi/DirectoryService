using System.Net;
using System.Net.Http.Json;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Common;
using DirectoryService.Contracts.Locations;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared;
using Shared.Errors;

namespace DirectoryService.IntegrationTests.Locations;

public class LocationEndpointTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task GetLocation_with_existing_id_should_succeed()
    {
        Guid locationId = await CreateLocationAsync("Get Location");

        HttpResponseMessage response = await Client.GetAsync($"api/locations/{locationId}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<GetLocationDto>>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(locationId, envelope.Result!.Id);
        Assert.Equal("Get Location", envelope.Result.Name);
    }

    [Fact]
    public async Task GetLocation_with_unknown_id_should_fail()
    {
        HttpResponseMessage response = await Client.GetAsync($"api/locations/{Guid.NewGuid()}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task GetLocations_with_valid_query_should_succeed()
    {
        await CreateLocationAsync("List Location");

        HttpResponseMessage response = await Client.GetAsync(
            "api/locations?SortBy=name&SortDir=asc&Pagination.Page=1&Pagination.PageSize=10");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<GetLocationsDto>>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.NotNull(envelope.Result);
        Assert.True(envelope.Result.totalCount >= envelope.Result.Locations.Count);
    }

    [Fact]
    public async Task GetLocations_with_invalid_query_should_fail()
    {
        HttpResponseMessage response = await Client.GetAsync("api/locations?SortBy=unknown");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.VALIDATION);
    }

    [Fact]
    public async Task GetTopLocations_should_succeed()
    {
        await CreateLocationAsync("Top Location");

        HttpResponseMessage response = await Client.GetAsync("api/locations/top");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<List<LocationTopDto>>>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.NotNull(envelope.Result);
        Assert.True(envelope.Result.Count <= 5);
        Assert.All(envelope.Result, location => Assert.NotNull(location.Address));
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

    [Fact]
    public async Task DeleteLocation_with_existing_id_should_succeed()
    {
        Guid locationId = await CreateLocationAsync("Deleted Location");

        HttpResponseMessage response = await Client.DeleteAsync($"api/locations/{locationId}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        var location = await ExecuteInDbAsync(async dbContext =>
            await dbContext
                .LocationsRead
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == locationId));

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(locationId, envelope.Result);
        Assert.NotNull(location);
        Assert.True(location.IsDeleted);
    }

    [Fact]
    public async Task DeleteLocation_with_unknown_id_should_fail()
    {
        HttpResponseMessage response = await Client.DeleteAsync($"api/locations/{Guid.NewGuid()}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }
}
