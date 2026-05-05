using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Departments;

public record UpdateLocationsCommand(Guid DepartmentId, UpdateLocationsRequest Request) : ICommand;