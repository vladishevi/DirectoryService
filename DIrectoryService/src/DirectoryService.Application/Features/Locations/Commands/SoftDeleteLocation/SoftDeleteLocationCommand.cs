using System.Windows.Input;
using ICommand = DirectoryService.Application.Abstractions.ICommand;

namespace DirectoryService.Application.Features.Locations.Commands.SoftDeleteLocation;

public record SoftDeleteLocationCommand(Guid Id) : ICommand;