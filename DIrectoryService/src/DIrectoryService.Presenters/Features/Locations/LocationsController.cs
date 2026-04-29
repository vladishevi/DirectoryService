using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Features.Locations;
using DirectoryService.Contracts.Locations;
using DirectoryService.Presenters.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters.Features.Locations;

[ApiController]
[Route("/api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> createLocationHandler,
        [FromBody] CreateLocationRequest createLocationRequest,
        CancellationToken cancellationToken)
    {
        CreateLocationCommand createLocationCommand = new(createLocationRequest);
        return await createLocationHandler.Handle(createLocationCommand, cancellationToken);
    }
}