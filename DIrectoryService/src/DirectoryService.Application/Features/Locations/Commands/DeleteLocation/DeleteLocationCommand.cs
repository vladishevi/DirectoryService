using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Locations;

public record DeleteLocationCommand(Guid Id) : ICommand;
