using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Errors;

namespace DirectoryService.IntegrationTests.Locations;

public class DeleteLocationTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task DeleteLocation_with_existing_id_should_succeed()
    {
        Guid locationId = await CreateLocationAsync("Deleted Location");

        HttpResponseMessage response = await Client.DeleteAsync($"api/locations/{locationId}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        var location = await ExecuteInDbAsync(async dbContext =>
            await dbContext
                .LocationsRead
                .FirstOrDefaultAsync(l => l.Id == locationId));
        
        var locationIgnoreFilters = await ExecuteInDbAsync(async dbContext =>
            await dbContext
                .LocationsRead
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == locationId));

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(locationId, envelope.Result);
        Assert.Null(location);
        Assert.NotNull(locationIgnoreFilters);
        Assert.True(locationIgnoreFilters.IsDeleted);
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