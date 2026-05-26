using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Departments;

public record DeleteLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;