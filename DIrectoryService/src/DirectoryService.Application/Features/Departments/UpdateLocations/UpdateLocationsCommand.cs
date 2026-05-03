using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Departments;

public record UpdateLocationsCommand(UpdateLocationsRequest Request) : ICommand;