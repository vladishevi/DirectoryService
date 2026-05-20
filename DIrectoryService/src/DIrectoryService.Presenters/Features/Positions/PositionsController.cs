using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Features.Positions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Presenters.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters.Features.Positions;

[ApiController]
[Route("/api/positions")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreatePositionCommand> handler,
        [FromBody] CreatePositionRequest request,
        CancellationToken cancellationToken)
    {
        CreatePositionCommand command = new(request);
        return await handler.Handle(command, cancellationToken);
    }

    [HttpPatch("{positionId}/name")]
    public async Task<EndpointResult<Guid>> UpdateName(
        [FromServices] ICommandHandler<Guid, UpdateNameCommand> handler,
        [FromBody] UpdateNameRequest request,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        UpdateNameCommand command = new(positionId, request);
        return await handler.Handle(command, cancellationToken);       
    }
}