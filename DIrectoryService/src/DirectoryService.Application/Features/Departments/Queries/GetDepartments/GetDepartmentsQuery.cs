using DirectoryService.Contracts.Departments;
using Shared.Core.Abstractions;

namespace DirectoryService.Application.Features.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery(GetDepartmentsRequest Request) : IQuery;