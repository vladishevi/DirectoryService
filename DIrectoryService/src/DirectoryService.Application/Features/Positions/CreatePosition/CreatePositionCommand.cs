using DirectoryService.Contracts.Positions;
using Shared.Core.Abstractions;

namespace DirectoryService.Application.Features.Positions;

public record CreatePositionCommand(CreatePositionRequest Request) : ICommand;