using ICommand = Shared.Core.Abstractions.ICommand;

namespace DirectoryService.Application.Features.Locations.Commands.SoftDeleteLocation;

public record SoftDeleteLocationCommand(Guid Id) : ICommand;