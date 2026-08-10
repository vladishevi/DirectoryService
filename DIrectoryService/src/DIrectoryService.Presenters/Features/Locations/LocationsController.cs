using DirectoryService.Application.Features.Locations.Commands;
using DirectoryService.Application.Features.Locations.Commands.SoftDeleteLocation;
using DirectoryService.Application.Features.Locations.GetLocation;
using DirectoryService.Application.Features.Locations.GetLocations;
using DirectoryService.Contracts.Locations;
using DirectoryService.Presenters.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Core.Abstractions;

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
        [FromServices] ICommandHandler<Guid, SoftDeleteLocationCommand> handler,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        SoftDeleteLocationCommand command = new(locationId);
        return await handler.Handle(command, cancellationToken);
    }
    
    [HttpGet("{locationId:guid}")]
    public async Task<EndpointResult<GetLocationDto>> Get(
        [FromServices] IQueryHandler<GetLocationDto, GetLocationQuery> handler,
        [FromRoute] Guid locationId,
        CancellationToken ct)
    {
        GetLocationQuery query = new(locationId);
        return await handler.Handle(query, ct);
    }

    [HttpGet("top")]
    public async Task<EndpointResult<List<LocationTopDto>>> Get(
        [FromServices] IQueryHandler<List<LocationTopDto>> handler,
        CancellationToken ct)
    {
        return await handler.Handle(ct);
    }

    [HttpGet]
    public async Task<EndpointResult<GetLocationsDto>> Get(
        [FromServices] IQueryHandler<GetLocationsDto, GetLocationsQuery> handler,
        [FromQuery] GetLocationsRequest request,
        CancellationToken ct)
    {
        GetLocationsQuery query = new(request);
        return await handler.Handle(query, ct);
    }
}
