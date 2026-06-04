using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Locations;

public class GetLocationHandler(
    IReadDbContext readDbContext, 
    ILogger<GetLocationHandler> logger)
    : IQueryHandler<GetLocationDto, GetLocationQuery>
{

    public async Task<Result<GetLocationDto, Errors>> Handle(GetLocationQuery query, CancellationToken ctx)
    {
        GetLocationDto? locationDto = await readDbContext.LocationsRead
            .Where(l => l.Id == query.LocationId)
            .Include(l => l.Departments)
            .Select(l => new GetLocationDto
            {
                DepartmentsIds = l.Departments.Select(d => d.Id).ToList(),
                Id = l.Id,
                Name = l.Name.Value,
                City = l.Address.City,
                Street = l.Address.Street,
                Building = l.Address.Building,
                Postcode = l.Address.Postcode,
                Timezone = l.Timezone.Code,
                IsActive = l.IsActive,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken: ctx);

        if (locationDto == null)
        {
            return GeneralErrors.NotFound($"Location by id {query.LocationId} not found").ToErrors();
        }
        
        logger.LogInformation("Location {name} retrieved", locationDto.Name);
        return locationDto;
    }
}
