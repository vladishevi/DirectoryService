using System.Net.Http.Json;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task CreateDepartment_with_valid_data_should_succeed()
    {
        //arrange
        var createLocationRequest = new CreateLocationRequest("London Office",
            new AddressDto { City = "London", Street = "Baker Street", Building = 221, Postcode = "NW1" },
            "Europe/London");
        HttpResponseMessage createLocationResponse = await Client.PostAsJsonAsync("api/locations", createLocationRequest);
        Envelope<Guid> createLocationEnvelope =
            await createLocationResponse.Content.ReadFromJsonAsync<Envelope<Guid>>();

        var request = new CreateDepartmentRequest("Engineering", "engineering", null, [createLocationEnvelope.Result]);

        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/departments", request);
        Envelope<Guid> envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();

        var department = await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.DepartmentsRead
                .Include(d => d.Locations)
                .FirstOrDefaultAsync(d => d.Id == envelope.Result);
        });

        //assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Null(envelope.Errors);
        Assert.NotEqual(Guid.Empty, envelope.Result);
        Assert.NotNull(department);
        Assert.Equal(request.Name, department.Name.Value);
        Assert.Equal(request.Identifier, department.Identifier.Value);
        Assert.Null(department.ParentDepartment);
        Assert.Equal(0, department.Depth);
        Assert.False(department.IsDeleted);
        Assert.Single(department.Locations);
        Assert.Contains(department.Locations, location => location.LocationId == createLocationEnvelope.Result);
    }

    [Fact]
    public async Task CreateDepartment_with_valid_parent_should_succeed()
    {
        //arrange
        var createParentLocationRequest = new CreateLocationRequest("London Office",
            new AddressDto { City = "London", Street = "Baker Street", Building = 221, Postcode = "NW1" },
            "Europe/London");
        HttpResponseMessage createParentLocationResponse =
            await Client.PostAsJsonAsync("api/locations", createParentLocationRequest);
        Envelope<Guid> createParentLocationEnvelope =
            await createParentLocationResponse.Content.ReadFromJsonAsync<Envelope<Guid>>();

        var createChildLocationRequest = new CreateLocationRequest("Manchester Office",
            new AddressDto { City = "Manchester", Street = "Deansgate", Building = 10, Postcode = "M1" },
            "Europe/London");
        HttpResponseMessage createChildLocationResponse =
            await Client.PostAsJsonAsync("api/locations", createChildLocationRequest);
        Envelope<Guid> createChildLocationEnvelope =
            await createChildLocationResponse.Content.ReadFromJsonAsync<Envelope<Guid>>();

        var createParentDepartmentRequest =
            new CreateDepartmentRequest("Engineering", "engineering", null, [createParentLocationEnvelope.Result]);
        HttpResponseMessage createParentDepartmentResponse =
            await Client.PostAsJsonAsync("api/departments", createParentDepartmentRequest);
        Envelope<Guid> createParentDepartmentEnvelope =
            await createParentDepartmentResponse.Content.ReadFromJsonAsync<Envelope<Guid>>();

        var request = new CreateDepartmentRequest("Platform", "platform", createParentDepartmentEnvelope.Result,
            [createChildLocationEnvelope.Result]);

        //act
        HttpResponseMessage response = await Client.PostAsJsonAsync("api/departments", request);
        Envelope<Guid> envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();

        var department = await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.DepartmentsRead
                .Include(d => d.ParentDepartment)
                .Include(d => d.Locations)
                .FirstOrDefaultAsync(d => d.Id == envelope.Result);
        });

        //assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Null(envelope.Errors);
        Assert.NotEqual(Guid.Empty, envelope.Result);
        Assert.NotNull(department);
        Assert.Equal(request.Name, department.Name.Value);
        Assert.Equal(request.Identifier, department.Identifier.Value);
        Assert.NotNull(department.ParentDepartment);
        Assert.Equal(createParentDepartmentEnvelope.Result, department.ParentDepartment.Id);
        Assert.Equal(1, department.Depth);
        Assert.Equal("engineering.platform", department.Path.Value);
        Assert.False(department.IsDeleted);
        Assert.Single(department.Locations);
        Assert.Contains(department.Locations, location => location.LocationId == createChildLocationEnvelope.Result);
    }
}
