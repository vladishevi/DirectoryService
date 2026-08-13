using Shared.Core.Abstractions;

namespace DirectoryService.Application.Features.Positions;

public record DeletePositionCommand(Guid Id) : ICommand;