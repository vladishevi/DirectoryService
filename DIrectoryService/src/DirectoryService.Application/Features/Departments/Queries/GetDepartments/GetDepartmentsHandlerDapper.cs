using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Departments;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsHandlerDapper(
    IDbConnectionFactory dbConnectionFactory,
    ILogger<GetDepartmentsHandlerDapper> logger) 
    : IQueryHandler<GetDepartmentsDto, GetDepartmentsQuery>
{
    public async Task<Result<GetDepartmentsDto, Errors>> Handle(GetDepartmentsQuery query, CancellationToken ct)
    {
        using IDbConnection connection = await dbConnectionFactory.CreateConnectionAsync(ct);

        var parameters = new DynamicParameters();
        parameters.Add("limit", query.Request.Pagination.PageSize, DbType.Int32);
        parameters.Add("offset", query.Request.Pagination.Page - 1, DbType.Int32);

        List<string> conditions = null;
        if (query.Request.Search != null)
        {
            conditions ??= [];
            conditions.Add("name ILIKE '%' || @search || '%'");
            parameters.Add("search", query.Request.Search, DbType.String);
        }

        string whereClause = conditions != null ? "WHERE " + string.Join(" AND ", conditions) : "";
        string command = $"""
                         SELECT id, name, path, created_at, COUNT(*) OVER() AS total
                         FROM departments
                         {whereClause}
                         ORDER BY name  
                         LIMIT @limit OFFSET @offset;
                         """;

        long? totalCount = null;
        List<DepartmentListItemDto> itemsDto = await connection.QueryAsync<DepartmentListItemDto, long, DepartmentListItemDto>(command, 
                map: (dep, count) =>
                {
                    totalCount ??= count;
                    return dep;
                },
                splitOn: "total",
                param: parameters)
            as List<DepartmentListItemDto>;

        return new GetDepartmentsDto(itemsDto, totalCount ?? 0 );
    }
}