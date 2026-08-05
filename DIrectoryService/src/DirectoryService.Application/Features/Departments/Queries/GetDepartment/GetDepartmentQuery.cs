using DirectoryService.Application.Abstractions;
using Shared.Core.Abstractions;

namespace DirectoryService.Application.Features.Departments.Queries.GetDepartment;

public record GetDepartmentQuery(Guid DepartmentId) : IQuery;