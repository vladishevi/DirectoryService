using DirectoryService.Application.Features.Departments;
using DirectoryService.Contracts.Departments;
using DirectoryService.Application.Abstractions;
using DirectoryService.Presenters.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters.Features.Departments;

[ApiController]
[Route("/api/departments")]
public class DepartmentsController
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] CreateDepartmentHandler createDepartmentHandler,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        CreateDepartmentCommand command = new(request);
        return await createDepartmentHandler.Handle(command, cancellationToken);
    }

    [HttpPatch("{departmentId}/locations")]
    public async Task<EndpointResult<Guid>> UpdateLocations(
        [FromServices] UpdateLocationsHandler updateLocationsHandler,
        [FromBody] UpdateLocationsRequest request,
        [FromRoute] Guid departmentId,
        CancellationToken cancellationToken)
    {
        UpdateLocationsCommand command = new(departmentId, request);
        return await updateLocationsHandler.Handle(command, cancellationToken);
    }

    [HttpPatch("{departmentId}/positions")]
    public async Task<EndpointResult<Guid>> UpdatePositions(
        [FromServices] UpdatePositionsHandler updatePositionsHandler,
        [FromBody] UpdatePositionsRequest request,
        [FromRoute] Guid departmentId,
        CancellationToken cancellationToken)
    {
        UpdatePositionsCommand command = new(departmentId, request);
        return await updatePositionsHandler.Handle(command, cancellationToken);
    }

    [HttpPut("{departmentId}/parent")]
    public async Task<EndpointResult<Guid>> UpdateParent(
        [FromServices] UpdateParentHandler updateParentHandler,
        [FromRoute] Guid departmentId,
        [FromBody] UpdateParentRequest request, 
        CancellationToken cancellationToken)
    {
        UpdateParentCommand command = new(departmentId, request);
        return await updateParentHandler.Handle(command, cancellationToken);
    }

    [HttpDelete("{departmentId}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromServices] ICommandHandler<Guid, DeleteDepartmentCommand> handler,
        [FromRoute] Guid departmentId,
        CancellationToken cancellationToken)
    {
        DeleteDepartmentCommand command = new(departmentId);
        return await handler.Handle(command, cancellationToken);
    }
}
