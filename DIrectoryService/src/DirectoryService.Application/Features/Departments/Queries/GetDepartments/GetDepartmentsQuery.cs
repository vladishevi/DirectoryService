using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Features.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery(GetDepartmentsRequest Request) : IQuery;