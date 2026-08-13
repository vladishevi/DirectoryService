using DirectoryService.Contracts.Departments;
using Shared.Core.Abstractions;

namespace DirectoryService.Application.Features.Departments.Commands;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : ICommand;
