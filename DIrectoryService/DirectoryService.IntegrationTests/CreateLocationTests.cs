using DirectoryService.Application.Features.Locations.Commands;
using DirectoryService.Contracts.Locations;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests;

public class CreateLocationTests : IClassFixture<DirectoryServiceTestWebFactory>, IAsyncLifetime
{
    private readonly IServiceProvider _services;
    private readonly DirectoryServiceTestWebFactory _webFactory;

    public CreateLocationTests(DirectoryServiceTestWebFactory factory)
    {
        _services = factory.Services;
        _webFactory = factory;
    }
    
    [Fact]
    public async void CreateLocation_with_valid_data_should_succeed()
    {
        //arrange
        var ct = CancellationToken.None;
        var command = new CreateLocationCommand(new CreateLocationRequest("My locfatgion test", new AddressDto
        {
            City = "my city",
            Street = "my strgeet",
            Building = 1,
            Postcode = "92"
        }, "Europe/London"));
        
        //act
        var result = await ExecuteHandler(async sut => await sut.Handle(command, ct));
        var location = await ExecuteInDb(async dbContext =>
        {
            return await dbContext.LocationsRead.FirstOrDefaultAsync(l => l.Id == result.Value, ct);
        });

        //assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);       
        Assert.NotEqual(Guid.Empty, result.Value);
        Assert.NotNull(location);
    }
    
    [Fact]
    public async void CreateLocation_with_invalid_timezone_should_fail()
    {
        //arrange
        var ct = CancellationToken.None;
        var command = new CreateLocationCommand(new CreateLocationRequest("My locfatgion test", new AddressDto
        {
            City = "my city",
            Street = "my strgeet",
            Building = 1,
            Postcode = "92"
        }, ""));
        
        //act
        var result = await ExecuteHandler(async sut => await sut.Handle(command, ct));
        bool anyLocationExists = await ExecuteInDb(async dbContext => await dbContext.LocationsRead.AnyAsync(cancellationToken: ct));

        //assert
        Assert.True(result.IsFailure);       
        Assert.NotEmpty(result.Error);
        Assert.False(anyLocationExists);
    }

    private async Task<T> ExecuteHandler<T>(Func<CreateLocationHandler, Task<T>> action)
    {
        await using var scope = _services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<CreateLocationHandler>();
        return await action(sut);
    }
    
    private async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        await using var scope = _services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        return await action(sut);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _webFactory.ResetDbAsync();       
    }
}