using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Departments.Commands;

public record DeleteDepartmentCommand(Guid Id) : ICommand;
