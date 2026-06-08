using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Locations.GetTopLocations;

public class GetTopLocationsHandler(
    IReadDbContext readDbContext,
    ILogger<GetTopLocationsHandler> logger)
    : IQueryHandler<DepartmentsCountDto[]>
{
    public async Task<Result<DepartmentsCountDto[], Errors>> Handle(CancellationToken cancellationToken)
    {
        try
        {
            return await readDbContext.LocationsRead
                .Include(l => l.Departments)
                .OrderByDescending(x => x.Departments.Count)
                .Take(5)
                .Select(l => new DepartmentsCountDto(l.Id, l.Name.Value, l.Departments.Count))
                .ToArrayAsync(cancellationToken: cancellationToken);
        }
        catch (TaskCanceledException)
        {
            logger.LogWarning("Operation cancelled while getting top locations");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception)
        {
            logger.LogError("Database error while getting top locations");
            return GeneralErrors.DatabaseError().ToErrors();
        }       
    }
}