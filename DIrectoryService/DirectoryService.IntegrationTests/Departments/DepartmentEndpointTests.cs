using System.Net;
using System.Net.Http.Json;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Common;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared;
using Shared.Errors;

namespace DirectoryService.IntegrationTests.Departments;

public class DepartmentEndpointTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task CreateDepartment_with_unknown_location_should_fail()
    {
        var request = new CreateDepartmentRequest("Department", "department", null, [Guid.NewGuid()]);

        HttpResponseMessage response = await Client.PostAsJsonAsync("api/departments", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task UpdateLocations_with_existing_department_and_locations_should_succeed()
    {
        Guid firstLocationId = await CreateLocationAsync("First Location");
        Guid secondLocationId = await CreateLocationAsync("Second Location");
        Guid departmentId = await CreateDepartmentAsync([firstLocationId]);
        var request = new UpdateLocationsRequest([secondLocationId]);

        HttpResponseMessage response = await Client.PatchAsJsonAsync($"api/departments/{departmentId}/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        var locationIds = await ExecuteInDbAsync(async dbContext =>
            await dbContext.Set<DepartmentLocation>()
                .Where(dl => dl.DepartmentId == departmentId)
                .Select(dl => dl.LocationId)
                .ToListAsync());

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(departmentId, envelope.Result);
        Assert.Equal([secondLocationId], locationIds);
    }

    [Fact]
    public async Task UpdateLocations_with_unknown_department_should_fail()
    {
        var request = new UpdateLocationsRequest([Guid.NewGuid()]);

        HttpResponseMessage response = await Client.PatchAsJsonAsync($"api/departments/{Guid.NewGuid()}/locations", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task UpdatePositions_with_existing_department_and_positions_should_succeed()
    {
        Guid departmentId = await CreateDepartmentAsync();
        Guid positionId = await CreatePositionAsync([departmentId]);
        var request = new UpdatePositionsRequest([positionId]);

        HttpResponseMessage response = await Client.PatchAsJsonAsync($"api/departments/{departmentId}/positions", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        var positionIds = await ExecuteInDbAsync(async dbContext =>
            await dbContext.Set<DepartmentPosition>()
                .Where(dp => dp.DepartmentId == departmentId)
                .Select(dp => dp.PositionId)
                .ToListAsync());

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(departmentId, envelope.Result);
        Assert.Equal([positionId], positionIds);
    }

    [Fact]
    public async Task UpdatePositions_with_unknown_department_should_fail()
    {
        var request = new UpdatePositionsRequest([Guid.NewGuid()]);

        HttpResponseMessage response = await Client.PatchAsJsonAsync($"api/departments/{Guid.NewGuid()}/positions", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task DeleteDepartment_with_existing_id_should_succeed()
    {
        Guid departmentId = await CreateDepartmentAsync();

        HttpResponseMessage response = await Client.DeleteAsync($"api/departments/{departmentId}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        bool departmentExists = await ExecuteInDbAsync(async dbContext =>
            await dbContext.DepartmentsRead.AnyAsync(d => d.Id == departmentId));
        
        var departmetmentIgnoreFilters = await ExecuteInDbAsync(async dbContext =>
            await dbContext
                .DepartmentsRead
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == departmentId));

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(departmentId, envelope.Result);
        Assert.False(departmentExists);
        Assert.NotNull(departmetmentIgnoreFilters);
        Assert.True(departmetmentIgnoreFilters.IsDeleted);
    }

    [Fact]
    public async Task DeleteDepartment_with_unknown_id_should_fail()
    {
        HttpResponseMessage response = await Client.DeleteAsync($"api/departments/{Guid.NewGuid()}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task DeleteDepartmentLocation_with_existing_location_should_succeed()
    {
        Guid locationId = await CreateLocationAsync();
        Guid departmentId = await CreateDepartmentAsync([locationId]);

        HttpResponseMessage response = await Client.DeleteAsync($"api/departments/{departmentId}/locations/{locationId}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        var locationIds = await ExecuteInDbAsync(async dbContext =>
            await dbContext.Set<DepartmentLocation>()
                .Where(dl => dl.DepartmentId == departmentId)
                .Select(dl => dl.LocationId)
                .ToListAsync());

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(departmentId, envelope.Result);
        Assert.Empty(locationIds);
    }

    [Fact]
    public async Task DeleteDepartmentLocation_with_unknown_department_should_fail()
    {
        HttpResponseMessage response = await Client.DeleteAsync($"api/departments/{Guid.NewGuid()}/locations/{Guid.NewGuid()}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task GetDepartment_with_existing_id_should_succeed()
    {
        Guid departmentId = await CreateDepartmentAsync(name: "Get Department", identifier: "getdepartment");

        HttpResponseMessage response = await Client.GetAsync($"api/departments/{departmentId}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<GetDepartmentDto>>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(departmentId, envelope.Result!.Id);
        Assert.Equal("Get Department", envelope.Result.Name);
    }

    [Fact]
    public async Task GetDepartment_with_unknown_id_should_fail()
    {
        HttpResponseMessage response = await Client.GetAsync($"api/departments/{Guid.NewGuid()}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task GetDepartments_with_valid_query_should_succeed()
    {
        Guid departmentId = await CreateDepartmentAsync(name: "List Department", identifier: "listdepartment");

        HttpResponseMessage response = await Client.GetAsync(
            "api/departments?Search=List&SortBy=Name&SortDir=Asc&Pagination.Page=1&Pagination.PageSize=10");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<GetDepartmentsDto>>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(1, envelope.Result!.totalCount);
        Assert.Contains(envelope.Result.Departments, d => d.Id == departmentId);
    }

    [Fact]
    public async Task GetDepartments_with_invalid_query_should_fail()
    {
        HttpResponseMessage response = await Client.GetAsync("api/departments?SortBy=unknown");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.VALIDATION);
    }

    [Fact]
    public async Task GetDepartmentsDapper_with_valid_query_should_succeed()
    {
        await CreateDepartmentAsync(name: "Dapper Department", identifier: "dapperdepartment");

        HttpResponseMessage response = await Client.GetAsync("api/departments/dapper");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<GetDepartmentsDto>>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.NotNull(envelope.Result);
        Assert.True(envelope.Result.totalCount >= envelope.Result.Departments.Count);
    }

    [Fact]
    public async Task GetDepartmentsDapper_when_database_fails_should_fail()
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

        HttpResponseMessage response = await client.GetAsync("api/departments/dapper");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.FAILURE);
    }
}
