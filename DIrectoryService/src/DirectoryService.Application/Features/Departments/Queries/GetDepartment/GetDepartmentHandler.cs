using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Departments.Queries.GetDepartment;

public class GetDepartmentHandler(
    IReadDbContext readDbContext,
    ILogger<GetDepartmentHandler> logger) 
    : IQueryHandler<GetDepartmentDto, GetDepartmentQuery>
{
    public async Task<Result<GetDepartmentDto, Errors>> Handle(GetDepartmentQuery query, CancellationToken cancellationToken)
    {
        GetDepartmentDto? departmentDto = await readDbContext.DepartmentsRead
            .Where(d => d.Id == query.DepartmentId)
            .Include(d => d.Locations)
            .Include(d => d.Positions)
            .Include(d => d.ParentDepartment)
            .Select(d => new GetDepartmentDto
            {
                Id = d.Id,
                Name = d.Name.Value,
                Identifier = d.Identifier.Value,
                ParentDepartmentId = d.ParentDepartment == null ? null : d.ParentDepartment.Id,
                Path = d.Path.Value,
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                Locations = d.Locations.Select(l => l.Id),
                Positions = d.Positions.Select(p => p.Id)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (departmentDto == null)
        {
            logger.LogWarning("Department not found with id {id}", query.DepartmentId);
            return GeneralErrors.NotFound($"Department with id {query.DepartmentId} not found").ToErrors();
        }

        return departmentDto;
    }
}