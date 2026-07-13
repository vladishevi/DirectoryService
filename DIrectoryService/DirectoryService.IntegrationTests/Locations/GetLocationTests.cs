using System.Net;
using System.Net.Http.Json;
using DirectoryService.Contracts.Locations;
using Shared;
using Shared.Errors;

namespace DirectoryService.IntegrationTests.Locations;

public class GetLocationTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
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
        Assert.False(envelope.Result.IsDeleted);
    }
    
    [Fact]
    public async Task GetLocation_deleted_should_fail()
    {
        Guid locationId = await CreateLocationAsync("Get Location");
        await Client.DeleteAsync($"api/locations/{locationId}");

        HttpResponseMessage response = await Client.GetAsync($"api/locations/{locationId}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
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
}