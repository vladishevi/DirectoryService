using DirectoryService.Application.Features.Departments;
using DirectoryService.Contracts.Departments;
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
}