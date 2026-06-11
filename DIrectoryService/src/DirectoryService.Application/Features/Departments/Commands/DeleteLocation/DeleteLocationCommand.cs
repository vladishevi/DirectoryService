using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Departments.Commands;

public record DeleteLocationCommand(Guid DepartmentId, Guid LocationId) : ICommand;