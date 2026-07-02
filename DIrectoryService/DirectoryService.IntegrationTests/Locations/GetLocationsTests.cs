using System.Net;
using System.Net.Http.Json;
using DirectoryService.Contracts.Locations;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Errors;

namespace DirectoryService.IntegrationTests.Locations;

public class GetLocationsTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task GetLocations_with_valid_query_should_succeed()
    {
        await CreateLocationAsync("List Location");

        HttpResponseMessage response = await Client.GetAsync(
            "api/locations?SortBy=name&SortDir=asc&Pagination.Page=1&Pagination.PageSize=10");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<GetLocationsDto>>();

        var ids = envelope.Result.Locations.Select(l => l.Id);
        var locations = await ExecuteInDbAsync(async dbContext =>
            await dbContext
                .LocationsRead
                .Where(l => ids.Contains(l.Id))
                .ToListAsync());

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.NotNull(envelope.Result);
        Assert.True(envelope.Result.totalCount >= envelope.Result.Locations.Count);
        Assert.All(locations, l => Assert.False(l.IsDeleted));
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
}