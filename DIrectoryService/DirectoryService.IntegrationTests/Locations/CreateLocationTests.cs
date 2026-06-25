using System.Net.Http.Json;
using DirectoryService.Contracts.Locations;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Errors;

namespace DirectoryService.IntegrationTests.Locations;

public class CreateLocationTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task CreateLocation_with_valid_data_should_succeed()
    {
        //arrange
        var request = new CreateLocationRequest("My locfatgion test",
            new AddressDto { City = "my city", Street = "my strgeet", Building = 1, Postcode = "92" }, "Europe/London");
        
        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        Envelope<Guid> envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        
        var location = await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.LocationsRead.FirstOrDefaultAsync(l => l.Id == envelope.Result);
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
    public async Task CreateLocation_with_invalid_timezone_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My locfatgion test",
            new AddressDto { City = "my city", Street = "my strgeet", Building = 1, Postcode = "92" }, "");
        
        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();
        bool anyLocationExists = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.AnyAsync());

        //assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(envelope.Errors);
        Assert.False(envelope.IsSuccess);
        Assert.False(anyLocationExists);
    }
    
    [Fact]
    public async Task CreateLocation_with_invalid_address_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My locfatgion test",
            new AddressDto { City = "my city", Street = "", Building = -1, Postcode = "92" }, "Europe/London");
        
        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();
        bool anyLocationExists = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.AnyAsync());

        //assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(envelope.Errors);
        Assert.False(envelope.IsSuccess);
        Assert.False(anyLocationExists);
    }
    
    [Fact]
    public async Task CreateLocation_with_invalid_name_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("",
            new AddressDto { City = "my city", Street = "opop", Building = 21, Postcode = "92" }, "Europe/London");
        
        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();
        bool anyLocationExists = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.AnyAsync());

        //assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(envelope.Errors);
        Assert.False(envelope.IsSuccess);
        Assert.False(anyLocationExists);
    }
    
    [Fact]
    public async Task CreateLocation_with_existing_name_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My locfatgion test",
            new AddressDto { City = "my city", Street = "my strgeet", Building = 12, Postcode = "952" }, "Europe/London");
        
        //act
        await Client.PostAsJsonAsync("api/locations", request);
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();
        int count = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.CountAsync());

        //assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(envelope.Errors);
        Assert.False(envelope.IsSuccess);
        Assert.True(envelope.Errors.Any(e => e.Type == ErrorType.CONFLICT));
        Assert.True(count == 1);
    }
}