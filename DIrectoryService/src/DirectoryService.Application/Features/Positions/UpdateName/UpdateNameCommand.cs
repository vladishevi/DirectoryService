using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Positions;

namespace DirectoryService.Application.Features.Positions;

public record UpdateNameCommand(Guid Id, UpdateNameRequest Request) : ICommand;