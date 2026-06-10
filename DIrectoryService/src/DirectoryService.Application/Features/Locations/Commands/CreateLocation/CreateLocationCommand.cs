using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Features.Locations.Commands;

public record CreateLocationCommand(CreateLocationRequest CreateLocationRequest) : ICommand;