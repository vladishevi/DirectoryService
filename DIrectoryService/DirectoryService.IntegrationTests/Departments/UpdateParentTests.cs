using System.Net.Http.Json;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Departments;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateParentTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task UpdateParent_set_parent_to_null_should_succeed()
    {
        //arrange
        //create locations
        var request = new CreateLocationRequest("My locfatgion test",
            new AddressDto { City = "my city", Street = "my strgeet", Building = 1, Postcode = "92" }, "Europe/London");
        var createLocationEnvelope =
            await Client.PostAndReadAsJsonAsync<Envelope<Guid>, CreateLocationRequest>("api/locations", request);
        var firstLocationId = createLocationEnvelope.Result;
        
        request = new CreateLocationRequest("My locfatgion test second",
            new AddressDto { City = "my city", Street = "my second strgeet", Building = 1, Postcode = "92" }, "Europe/London");
        createLocationEnvelope =
            await Client.PostAndReadAsJsonAsync<Envelope<Guid>, CreateLocationRequest>("api/locations", request);
        var secondLocationId = createLocationEnvelope.Result;
        
        //create parent department
        var createParentDepartmentRequest = new CreateDepartmentRequest(Name: "Parent Department",
            Identifier: "parent", ParentId: null, LocationIds: [firstLocationId]);
        var createParentEnvelope =
            await Client.PostAndReadAsJsonAsync<Envelope<Guid>, CreateDepartmentRequest>("api/departments", createParentDepartmentRequest);
        var parentId = createParentEnvelope.Result;
        
        //create department
        var createDepartmentRequest = new CreateDepartmentRequest(Name: "Test Department",
            Identifier: "test", ParentId: parentId, LocationIds: [secondLocationId]);
        var createDepartmentEnvelope = await Client.PostAndReadAsJsonAsync<Envelope<Guid>, CreateDepartmentRequest>("api/departments",
            createDepartmentRequest);
        var departmentId = createDepartmentEnvelope.Result;
        var department = await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.DepartmentsRead
                .Include(d => d.ParentDepartment)
                .FirstAsync(d => d.Id == departmentId);
        });

        var updateParentRequest = new UpdateParentRequest(null);

        //act
        var response = await Client.PutAsJsonAsync($"api/departments/{department.Id}/parent", updateParentRequest);
        var updateParentEnvelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        department = await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.DepartmentsRead
                .Include(d => d.ParentDepartment)
                .FirstAsync(d => d.Id == departmentId);
        });
        
        //assert
        Assert.True(updateParentEnvelope.IsSuccess);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(updateParentEnvelope.Errors);
        Assert.Null(department.ParentDepartment);
        Assert.Equal(department.Depth, 0);
    }
    
    [Fact]
    public async Task UpdateParent_add_parent_should_succeed()
    {
        //arrange
        //create locations
        var request = new CreateLocationRequest("My locfatgion test",
            new AddressDto { City = "my city", Street = "my strgeet", Building = 1, Postcode = "92" }, "Europe/London");
        var createLocationEnvelope =
            await Client.PostAndReadAsJsonAsync<Envelope<Guid>, CreateLocationRequest>("api/locations", request);
        var firstLocationId = createLocationEnvelope.Result;
        
        request = new CreateLocationRequest("My locfatgion test second",
            new AddressDto { City = "my city", Street = "my second strgeet", Building = 1, Postcode = "92" }, "Europe/London");
        createLocationEnvelope =
            await Client.PostAndReadAsJsonAsync<Envelope<Guid>, CreateLocationRequest>("api/locations", request);
        var secondLocationId = createLocationEnvelope.Result;
        
        //create parent department
        var createParentDepartmentRequest = new CreateDepartmentRequest(Name: "Parent Department",
            Identifier: "parent", ParentId: null, LocationIds: [firstLocationId]);
        var createParentEnvelope =
            await Client.PostAndReadAsJsonAsync<Envelope<Guid>, CreateDepartmentRequest>("api/departments", createParentDepartmentRequest);
        var parentId = createParentEnvelope.Result;
        
        //create department
        var createDepartmentRequest = new CreateDepartmentRequest(Name: "Test Department",
            Identifier: "test", null, LocationIds: [secondLocationId]);
        var createDepartmentEnvelope = await Client.PostAndReadAsJsonAsync<Envelope<Guid>, CreateDepartmentRequest>("api/departments",
            createDepartmentRequest);
        var departmentId = createDepartmentEnvelope.Result;
        var department = await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.DepartmentsRead
                .Include(d => d.ParentDepartment)
                .FirstAsync(d => d.Id == departmentId);
        });

        var updateParentRequest = new UpdateParentRequest(createParentEnvelope.Result);

        //act
        var response = await Client.PutAsJsonAsync($"api/departments/{department.Id}/parent", updateParentRequest);
        var updateParentEnvelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        department = await ExecuteInDbAsync(async dbContext =>
        {
            return await dbContext.DepartmentsRead
                .Include(d => d.ParentDepartment)
                .FirstAsync(d => d.Id == departmentId);
        });
        
        //assert
        Assert.True(updateParentEnvelope.IsSuccess);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Null(updateParentEnvelope.Errors);
        Assert.NotNull(department.ParentDepartment);
        Assert.NotEqual(department.Depth, 0);
        Assert.True(department.ParentDepartment.Id == parentId);
    }
}