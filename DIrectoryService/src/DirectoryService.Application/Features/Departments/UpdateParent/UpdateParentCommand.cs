using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Features.Departments;

public record UpdateParentCommand(UpdateParentRequest Request) : ICommand;