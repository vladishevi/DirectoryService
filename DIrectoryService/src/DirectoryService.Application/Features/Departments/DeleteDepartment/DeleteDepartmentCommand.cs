using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Departments;

public record DeleteDepartmentCommand(Guid Id) : ICommand;
