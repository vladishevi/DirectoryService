using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions;
using Shared;

namespace DirectoryService.Application.Features.Positions;

public interface IPositionsRepository
{
    Task<Result<Guid, Errors>> Add(Position position, CancellationToken cancellationToken);   
}