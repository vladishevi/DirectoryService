using DirectoryService.Application.Abstractions;

namespace DirectoryService.Presenters.Features.Positions;

public record UpdateNameCommand(Guid Id, UpdateNameRequest Request) : ICommand;