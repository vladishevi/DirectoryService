using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Errors;

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
                                            SELECT l.id, l.name, COUNT(dl.department_id) departmentsCount, l.city, l.street, l.building, l.postcode
                                            FROM locations l
                                                LEFT JOIN department_locations dl ON l.id = dl.location_id
                                                LEFT JOIN departments d ON dl.department_id =  d.id
                                                WHERE d.is_deleted IS NOT TRUE
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
            logger.LogError(ex, "Database error while getting top locations");
            return GeneralErrors.DatabaseError().ToErrors();
        }
    }
}
