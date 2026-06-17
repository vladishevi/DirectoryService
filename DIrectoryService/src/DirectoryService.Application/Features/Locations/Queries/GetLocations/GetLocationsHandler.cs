using System.Data;
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

            List<string> conditions = [];

            DynamicParameters parameters = new();
            parameters.Add("limit", query.Request.Pagination.PageSize, DbType.Int32);
            parameters.Add("offset", query.Request.Pagination.Page - 1, DbType.Int32);

            if (query.Request.Search != null)
            {
                conditions.Add("name ILIKE '%' || @search || '%'");
                parameters.Add("search", query.Request.Search, DbType.String);
            }

            string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";
            string command = $"""
                              SELECT locations.id, locations.name, created_at, count(department_locations.department_id) departmentsCount,
                                     locations.city, locations.street, locations.building, locations.postcode, count(*) OVER () as totalCount
                              FROM locations
                              LEFT JOIN department_locations ON locations.id = department_locations.location_id
                              {whereClause}
                              GROUP BY locations.id, locations.name, locations.city, locations.street, locations.building, locations.postcode, created_at
                              HAVING COUNT(department_locations.department_id) >= {query.Request.MinDepartmentCount}
                              ORDER BY {query.Request.SortBy.ToLowerInvariant()} {query.Request.SortDir}
                              LIMIT @limit OFFSET @offset;
                              """;
            
            using var connection = await dbConnectionFactory.CreateConnectionAsync(ct);
            long? totalCount = null;
            List<GetLocationsItemDto> itemDto = await connection.QueryAsync<GetLocationsItemDto, AddressDto, long, GetLocationsItemDto>(command,
                    map: (location, address, count) =>
                    {
                        totalCount ??= count;
                        return location with { Address = address };
                    },
                    splitOn: "city, totalCount",
                    param: parameters) 
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