using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Departments.Queries.GetDepartment;

public record GetDepartmentQuery(Guid DepartmentId) : IQuery;