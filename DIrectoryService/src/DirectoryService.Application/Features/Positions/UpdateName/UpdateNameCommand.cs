using DirectoryService.Contracts.Positions;
using Shared.Core.Abstractions;

namespace DirectoryService.Application.Features.Positions;

public record UpdateNameCommand(Guid Id, UpdateNameRequest Request) : ICommand;