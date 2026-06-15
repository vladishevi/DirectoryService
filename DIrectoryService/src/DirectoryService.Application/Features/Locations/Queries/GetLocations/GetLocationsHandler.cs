using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Locations.GetLocations;

public class GetLocationsHandler(
    IDbConnectionFactory dbConnectionFactory,
    IValidator<GetLocationsQuery> validator,
    ILogger<GetLocationsHandler> logger)
    : IQueryHandler<GetLocationsDto, GetLocationsQuery>
{
    public async Task<Result<GetLocationsDto, Errors>> Handle(GetLocationsQuery query, CancellationToken ct)
    {
        try
        {
            //validation
            ValidationResult result = await validator.ValidateAsync(query, ct);
            if (!result.IsValid)
            {
                logger.LogError("Validation failed for GetDepartmentsQuery");
                return result.ToErrors();
            }

            string command = """
                             SELECT id, name, city, street, building, postcode, created_at, count(*) OVER () as total
                             FROM locations
                             """;
            using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
            long? totalCount = null;
            List<GetLocationsItemDto> itemDto = await connection.QueryAsync<GetLocationsItemDto, long, GetLocationsItemDto>(command,
                    map: (location, count) =>
                    {
                        totalCount ??= count;
                        return location;
                    },
                    splitOn: "total") 
                as List<GetLocationsItemDto>;
            return new GetLocationsDto(itemDto, totalCount ?? 0);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation cancelled while getting locations");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database error while getting locations");       
            return GeneralErrors.DatabaseError().ToErrors();       
        }
    }
}