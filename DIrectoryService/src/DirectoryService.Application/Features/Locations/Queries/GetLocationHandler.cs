using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
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
        Location? location = await readDbContext.LocationsRead
            .Where(l => l.Id == query.LocationId)
            .Include(l => l.Departments)
            .FirstOrDefaultAsync(cancellationToken: ctx);

        if (location == null)
        {
            return GeneralErrors.NotFound($"Location by id {query.LocationId} not found").ToErrors();
        }

        
        logger.LogInformation("Location {name} retrieved", location.Name);
        return new GetLocationDto
        {
            DepartmentsIds = location.Departments.Select(d => d.Id).ToList(),
            Id = location.Id,
            Name = location.Name.Value,
            City = location.Address.City,
            Street = location.Address.Street,
            Building = location.Address.Building,
            Postcode = location.Address.Postcode,
            Timezone = location.Timezone.Code,
            IsActive = location.IsActive,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }
}
