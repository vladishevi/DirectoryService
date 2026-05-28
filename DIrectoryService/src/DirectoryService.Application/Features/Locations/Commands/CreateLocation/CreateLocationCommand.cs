using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Features.Locations;

public record CreateLocationCommand(CreateLocationRequest CreateLocationRequest) : ICommand;