using System.Net;
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
        Assert.Equal(request.Name, location.Name.Value);
        Assert.Equal(request.Address.City, location.Address.City);
        Assert.Equal(request.Address.Street, location.Address.Street);
        Assert.Equal(request.Address.Building, location.Address.Building);
        Assert.Equal(request.Address.Postcode, location.Address.Postcode);
        Assert.Equal(request.Timezone, location.Timezone.Code);
        Assert.False(location.IsDeleted);
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
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.NotNull(envelope.Errors);
        Assert.Contains(envelope.Errors, e => e.Type == ErrorType.CONFLICT);
        Assert.True(count == 1);
    }

    [Fact]
    public async Task CreateLocation_with_existing_name_different_case_should_fail()
    {
        //arrange
        var firstRequest = new CreateLocationRequest("London Office",
            new AddressDto { City = "London", Street = "Baker Street", Building = 221, Postcode = "NW1" },
            "Europe/London");
        var duplicateRequest = new CreateLocationRequest("london office",
            new AddressDto { City = "London", Street = "Fleet Street", Building = 1, Postcode = "EC4" },
            "Europe/London");

        //act
        await Client.PostAsJsonAsync("api/locations", firstRequest);
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", duplicateRequest);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();
        int count = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.CountAsync());

        //assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.NotNull(envelope.Errors);
        Assert.Contains(envelope.Errors, e => e.Type == ErrorType.CONFLICT);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CreateLocation_with_existing_address_should_fail()
    {
        //arrange
        var firstRequest = new CreateLocationRequest("London Office",
            new AddressDto { City = "London", Street = "Baker Street", Building = 221, Postcode = "NW1" },
            "Europe/London");
        var duplicateRequest = new CreateLocationRequest("London Branch",
            new AddressDto { City = "London", Street = "Baker Street", Building = 221, Postcode = "NW1" },
            "Europe/London");

        //act
        await Client.PostAsJsonAsync("api/locations", firstRequest);
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", duplicateRequest);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();
        int count = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.CountAsync());

        //assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.NotNull(envelope.Errors);
        Assert.Contains(envelope.Errors, e => e.Type == ErrorType.CONFLICT);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CreateLocation_with_concurrent_duplicate_requests_should_create_only_one_location()
    {
        //arrange
        const int requestsCount = 8;

        var requests = Enumerable.Range(0, requestsCount)
            .Select(_ => new CreateLocationRequest("Race Location",
                new AddressDto { City = "London", Street = "Baker Street", Building = 221, Postcode = "NW1" },
                "Europe/London"))
            .ToList();

        //act
        HttpResponseMessage[] responses = await Task.WhenAll(
            requests.Select(request => Client.PostAsJsonAsync("api/locations", request)));

        Envelope<Guid>[] successEnvelopes = await Task.WhenAll(
            responses
                .Where(response => response.IsSuccessStatusCode)
                .Select(response => response.Content.ReadFromJsonAsync<Envelope<Guid>>()));

        Envelope<object>[] conflictEnvelopes = await Task.WhenAll(
            responses
                .Where(response => response.StatusCode == HttpStatusCode.Conflict)
                .Select(response => response.Content.ReadFromJsonAsync<Envelope<object>>()));

        int count = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.CountAsync());

        //assert
        Assert.Single(responses, response => response.IsSuccessStatusCode);
        Assert.Equal(requestsCount - 1, responses.Count(response => response.StatusCode == HttpStatusCode.Conflict));
        Assert.Single(successEnvelopes);
        Assert.All(successEnvelopes, envelope =>
        {
            Assert.NotNull(envelope);
            Assert.True(envelope.IsSuccess);
            Assert.Null(envelope.Errors);
            Assert.NotEqual(Guid.Empty, envelope.Result);
        });
        Assert.All(conflictEnvelopes, envelope =>
        {
            Assert.NotNull(envelope);
            Assert.False(envelope.IsSuccess);
            Assert.NotNull(envelope.Errors);
            Assert.Contains(envelope.Errors, e => e.Type == ErrorType.CONFLICT);
        });
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CreateLocation_with_two_character_name_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("ab",
            new AddressDto { City = "my city", Street = "my street", Building = 1, Postcode = "92" }, "Europe/London");

        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();
        bool anyLocationExists = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.AnyAsync());

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.NotNull(envelope.Errors);
        Assert.Contains(envelope.Errors, e => e.Type == ErrorType.VALIDATION);
        Assert.False(anyLocationExists);
    }

    [Fact]
    public async Task CreateLocation_with_three_character_name_should_succeed()
    {
        //arrange
        var request = new CreateLocationRequest("abc",
            new AddressDto { City = "my city", Street = "my street", Building = 1, Postcode = "92" }, "Europe/London");

        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        Envelope<Guid> envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        bool locationExists = await ExecuteInDbAsync(async dbContext =>
            await dbContext.LocationsRead.AnyAsync(l => l.Id == envelope.Result));

        //assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(envelope.Errors);
        Assert.True(envelope.IsSuccess);
        Assert.NotEqual(Guid.Empty, envelope.Result);
        Assert.True(locationExists);
    }

    [Fact]
    public async Task CreateLocation_with_120_character_name_should_succeed()
    {
        //arrange
        var request = new CreateLocationRequest(new string('a', 120),
            new AddressDto { City = "my city", Street = "my street", Building = 1, Postcode = "92" }, "Europe/London");

        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        Envelope<Guid> envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        bool locationExists = await ExecuteInDbAsync(async dbContext =>
            await dbContext.LocationsRead.AnyAsync(l => l.Id == envelope.Result));

        //assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(envelope.Errors);
        Assert.True(envelope.IsSuccess);
        Assert.NotEqual(Guid.Empty, envelope.Result);
        Assert.True(locationExists);
    }

    [Fact]
    public async Task CreateLocation_with_121_character_name_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest(new string('a', 121),
            new AddressDto { City = "my city", Street = "my street", Building = 1, Postcode = "92" }, "Europe/London");

        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();
        bool anyLocationExists = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.AnyAsync());

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.NotNull(envelope.Errors);
        Assert.Contains(envelope.Errors, e => e.Type == ErrorType.VALIDATION);
        Assert.False(anyLocationExists);
    }

    [Fact]
    public async Task CreateLocation_with_null_request_should_fail()
    {
        //arrange
        CreateLocationRequest request = null;

        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();
        bool anyLocationExists = await ExecuteInDbAsync(async dbContext => await dbContext.LocationsRead.AnyAsync());

        //assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.False(anyLocationExists);
    }

    [Fact]
    public async Task CreateLocation_with_null_address_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location", null, "Europe/London");

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
    public async Task CreateLocation_with_null_city_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = null, Street = "my street", Building = 1, Postcode = "92" }, "Europe/London");

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
    public async Task CreateLocation_with_empty_city_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = "", Street = "my street", Building = 1, Postcode = "92" }, "Europe/London");

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
    public async Task CreateLocation_with_null_postcode_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = "my city", Street = "my street", Building = 1, Postcode = null }, "Europe/London");

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
    public async Task CreateLocation_with_empty_postcode_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = "my city", Street = "my street", Building = 1, Postcode = "" }, "Europe/London");

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
    public async Task CreateLocation_with_invalid_timezone_format_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = "my city", Street = "my street", Building = 1, Postcode = "92" }, "Invalid/Timezone");

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
    public async Task CreateLocation_with_whitespace_only_name_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("   ",
            new AddressDto { City = "my city", Street = "my street", Building = 1, Postcode = "92" }, "Europe/London");

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
    public async Task CreateLocation_with_whitespace_only_city_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = "   ", Street = "my street", Building = 1, Postcode = "92" }, "Europe/London");

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
    public async Task CreateLocation_with_whitespace_only_street_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = "my city", Street = "   ", Building = 1, Postcode = "92" }, "Europe/London");

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
    public async Task CreateLocation_with_whitespace_only_postcode_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = "my city", Street = "my street", Building = 1, Postcode = "   " }, "Europe/London");

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
    public async Task CreateLocation_with_zero_building_number_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = "my city", Street = "my street", Building = 0, Postcode = "92" }, "Europe/London");

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
    public async Task CreateLocation_with_min_int_building_number_should_fail()
    {
        //arrange
        var request = new CreateLocationRequest("My location",
            new AddressDto { City = "my city", Street = "my street", Building = int.MinValue, Postcode = "92" }, "Europe/London");

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
}
