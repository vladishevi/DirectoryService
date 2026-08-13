using DirectoryService.Contracts.Locations;
using Shared.Core.Abstractions;

namespace DirectoryService.Application.Features.Locations.Commands;

public record CreateLocationCommand(CreateLocationRequest CreateLocationRequest) : ICommand;