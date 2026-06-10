using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Features.Departments.Commands;

public record UpdatePositionsCommand(Guid DepartmentId, UpdatePositionsRequest Request) : ICommand;
