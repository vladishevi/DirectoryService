using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Common;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsHandler(
    IReadDbContext readDbContext,
    IValidator<GetDepartmentsQuery> validator,
    ILogger<GetDepartmentsHandler> logger)
    : IQueryHandler<GetDepartmentsDto, GetDepartmentsQuery>
{
    public async Task<Result<GetDepartmentsDto, Errors>> Handle(GetDepartmentsQuery query, CancellationToken ct)
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
        
            IQueryable<Department> queryable = readDbContext.DepartmentsRead;
            if (!string.IsNullOrWhiteSpace(query.Request.Search)) 
                queryable = queryable.Where(d => d.Name.Value.Contains(query.Request.Search));

            queryable = query.Request.SortBy switch
            {
                "Name" => query.Request.SortDir == "Asc"
                    ? queryable.OrderBy(d => d.Name.Value)
                    : queryable.OrderByDescending(d => d.Name.Value),
                "CreatedAt" => query.Request.SortDir == "Asc"
                    ? queryable.OrderBy(d => d.CreatedAt)
                    : queryable.OrderByDescending(d => d.CreatedAt),
                _ => queryable
            };

            int totalCount = await queryable.CountAsync(ct);
        
            //pagination
            PaginationRequest pagination = query.Request.Pagination;
            queryable = queryable
                .Skip(pagination.PageSize * (pagination.Page - 1))
                .Take(pagination.PageSize);

            List<DepartmentListItemDto> departments = await queryable
                .Select(d => new DepartmentListItemDto
                {
                    Id = d.Id, Name = d.Name.Value, Path = d.Path.Value, CreatedAt = d.CreatedAt
                }).ToListAsync(ct);


            return new GetDepartmentsDto(departments, totalCount);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation cancelled while getting departments");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database error while getting departments");       
            return GeneralErrors.DatabaseError().ToErrors();
        }
    }
}