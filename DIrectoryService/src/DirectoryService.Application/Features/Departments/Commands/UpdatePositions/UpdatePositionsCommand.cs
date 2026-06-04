using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Features.Departments;

public record UpdatePositionsCommand(Guid DepartmentId, UpdatePositionsRequest Request) : ICommand;
