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

    [HttpDelete("{locationId}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromServices] ICommandHandler<Guid, DeleteLocationCommand> handler,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        DeleteLocationCommand command = new(locationId);
        return await handler.Handle(command, cancellationToken);
    }
}