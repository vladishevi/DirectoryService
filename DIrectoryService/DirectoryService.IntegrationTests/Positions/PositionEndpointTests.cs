using System.Net;
using System.Net.Http.Json;
using DirectoryService.Contracts.Positions;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Errors;

namespace DirectoryService.IntegrationTests.Positions;

public class PositionEndpointTests(DirectoryServiceTestWebFactory factory) : DirectoryServiceTests(factory)
{
    [Fact]
    public async Task CreatePosition_with_valid_data_should_succeed()
    {
        Guid departmentId = await CreateDepartmentAsync();
        var request = new CreatePositionRequest("Software Engineer", "Builds software", [departmentId]);

        HttpResponseMessage response = await Client.PostAsJsonAsync("api/positions", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        var position = await ExecuteInDbAsync(async dbContext =>
            await dbContext.Positions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == envelope!.Result));
        var departmentIds = await ExecuteInDbAsync(async dbContext =>
            await dbContext.Set<DepartmentPosition>()
                .Where(dp => dp.PositionId == envelope!.Result)
                .Select(dp => dp.DepartmentId)
                .ToListAsync());

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.NotEqual(Guid.Empty, envelope.Result);
        Assert.NotNull(position);
        Assert.Equal(request.Name, position.Name.Value);
        Assert.Equal([departmentId], departmentIds);
    }

    [Fact]
    public async Task CreatePosition_with_empty_department_ids_should_fail()
    {
        var request = new CreatePositionRequest("Software Engineer", "Builds software", []);

        HttpResponseMessage response = await Client.PostAsJsonAsync("api/positions", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.VALIDATION);
    }

    [Fact]
    public async Task UpdatePositionName_with_existing_position_should_succeed()
    {
        Guid positionId = await CreatePositionAsync();
        var request = new UpdateNameRequest("Principal Engineer");

        HttpResponseMessage response = await Client.PatchAsJsonAsync($"api/positions/{positionId}/name", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        var positionName = await ExecuteInDbAsync(async dbContext =>
            await dbContext.Positions.AsNoTracking()
                .Where(p => p.Id == positionId)
                .Select(p => p.Name.Value)
                .FirstAsync());

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(positionId, envelope.Result);
        Assert.Equal(request.Name, positionName);
    }

    [Fact]
    public async Task UpdatePositionName_with_unknown_position_should_fail()
    {
        var request = new UpdateNameRequest("Principal Engineer");

        HttpResponseMessage response = await Client.PatchAsJsonAsync($"api/positions/{Guid.NewGuid()}/name", request);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task DeletePosition_with_existing_position_should_succeed()
    {
        Guid positionId = await CreatePositionAsync();

        HttpResponseMessage response = await Client.DeleteAsync($"api/positions/{positionId}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid>>();
        var position = await ExecuteInDbAsync(async dbContext =>
            await dbContext.Positions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == positionId));
        var positionIgnoreFilters = await ExecuteInDbAsync(async dbContext =>
            await dbContext.Positions
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == positionId));

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(envelope);
        Assert.True(envelope.IsSuccess);
        Assert.Equal(positionId, envelope.Result);
        Assert.Null(position);
        Assert.NotNull(positionIgnoreFilters);
        Assert.True(positionIgnoreFilters.IsDeleted);
    }

    [Fact]
    public async Task DeletePosition_with_unknown_position_should_fail()
    {
        HttpResponseMessage response = await Client.DeleteAsync($"api/positions/{Guid.NewGuid()}");
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(envelope);
        Assert.False(envelope.IsSuccess);
        Assert.Contains(envelope.Errors!, e => e.Type == ErrorType.NOT_FOUND);
    }
}
