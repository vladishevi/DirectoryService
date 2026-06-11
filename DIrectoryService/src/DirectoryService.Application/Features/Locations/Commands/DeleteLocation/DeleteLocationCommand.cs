using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Features.Locations.Commands;

public record DeleteLocationCommand(Guid Id) : ICommand;
