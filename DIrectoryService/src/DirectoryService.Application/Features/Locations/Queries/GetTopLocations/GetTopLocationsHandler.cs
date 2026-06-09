using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Locations.GetTopLocations;

public class GetTopLocationsHandler(
    IReadDbContext readDbContext,
    IDbConnectionFactory dbConnectionFactory,
    ILogger<GetTopLocationsHandler> logger)
    : IQueryHandler<List<LocationTopDto>>
{
    public async Task<Result<List<LocationTopDto>, Errors>> Handle(CancellationToken ct)
    {
        try
        {
            using IDbConnection dbConnection = await dbConnectionFactory.CreateConnectionAsync(ct);
            CommandDefinition command = new("""
                        SELECT l.id, l.name, COUNT(dp.department_id) departmentsCount, l.city, l.street, l.building, l.postcode
                        FROM locations l
                            LEFT JOIN department_locations dp ON l.id = dp.location_id
                        GROUP BY l.id, l.name, l.city, l.street, l.building, l.postcode
                        ORDER BY departmentsCount DESC
                        LIMIT 5
                        """,
                cancellationToken: ct);

            IEnumerable<LocationTopDto> dto =
                await dbConnection.QueryAsync<LocationTopDto, AddressDto, LocationTopDto>(
                    command,
                    (l, a) => l with { Address = a },
                    splitOn: "city");

            return dto.ToList();

        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation cancelled while getting top locations");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception ex)
        {
            logger.LogError($"Database error while getting top locations. {ex.Message}");
            return GeneralErrors.DatabaseError().ToErrors();
        }
    }
}
