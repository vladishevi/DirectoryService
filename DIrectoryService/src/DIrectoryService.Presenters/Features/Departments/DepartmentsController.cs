using DirectoryService.Application.Features.Departments;
using DirectoryService.Contracts.Departments;
using DirectoryService.Presenters.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using Shared;

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
}