using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters.Locations;

[ApiController]
[Route("/api/locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromServices] LocationsService locationsService,
        [FromBody] CreateLocationDto createLocationDto,
        CancellationToken cancellationToken)
    {
        Result<Guid, string> result = await locationsService.Create(createLocationDto, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}