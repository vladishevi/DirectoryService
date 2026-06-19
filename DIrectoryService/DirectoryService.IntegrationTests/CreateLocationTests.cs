using DirectoryService.Application.Features.Locations.Commands;
using DirectoryService.Contracts.Locations;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests;

public class CreateLocationTests : IClassFixture<DirectoryServiceTestWebFactory>
{
    private readonly DirectoryServiceTestWebFactory _factory;
    private readonly IServiceProvider _services;

    public CreateLocationTests(DirectoryServiceTestWebFactory factory)
    {
        _services = factory.Services;
    }
    
    [Fact]
    public async void CreateLocation_with_valid_data_should_return_200()
    {
        //arrange
        await using var scope = _services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<CreateLocationHandler>();
        var ct = CancellationToken.None;
        var command = new CreateLocationCommand(new CreateLocationRequest("My locfatgion test", new AddressDto
        {
            City = "my city",
            Street = "my strgeet",
            Building = 1,
            Postcode = "92"
        }, "Europe/London"));
        
        //act
        var result = await sut.Handle(command, ct);

        //assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);       
        Assert.NotEqual(Guid.Empty, result.Value);
    }
}