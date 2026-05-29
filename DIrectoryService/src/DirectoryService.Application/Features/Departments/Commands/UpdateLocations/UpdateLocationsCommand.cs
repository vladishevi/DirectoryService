using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Features.Departments;

public record UpdateLocationsCommand(Guid DepartmentId, UpdateLocationsRequest Request) : ICommand;