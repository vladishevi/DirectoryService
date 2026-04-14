using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Contracts.Locations;
using DirectoryService.Presenters.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DirectoryService.Presenters.Locations;

[ApiController]
[Route("/api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] LocationsService locationsService,
        [FromBody] CreateLocationRequest createLocationRequest,
        CancellationToken cancellationToken)
    {
        Result<Guid, Errors> result = await locationsService.Create(createLocationRequest, cancellationToken);
        EndpointResult actionResult = result.IsFailure ? EndpointResult.Error(result.Error) : EndpointResult.Success(result.Value);
        return actionResult;
    }
}