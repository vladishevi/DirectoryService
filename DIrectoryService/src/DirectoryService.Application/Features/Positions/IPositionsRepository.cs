using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Positions;
using Shared.Errors;

namespace DirectoryService.Application.Features.Positions;

public interface IPositionsRepository : IRepository
{
    Task<Result<Guid, Errors>> Add(Position position, CancellationToken cancellationToken);
    Task<Result<Position, Errors>> GetById(Guid id, CancellationToken ct);
    Task<Result<bool, Errors>> AllExist(IEnumerable<Guid> ids, bool active, CancellationToken cancellationToken);
    Task<Result<Guid, Errors>> Delete(Position position);
}
