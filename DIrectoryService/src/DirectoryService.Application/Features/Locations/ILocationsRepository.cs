using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Shared;

namespace DirectoryService.Application.Features.Locations;

public interface ILocationsRepository : IRepository
{
    Task<Result<Guid, Errors>> Add(Location location, CancellationToken cancellationToken);
    Task<Result<Location, Errors>> GetById(Guid id, CancellationToken cancellationToken);
    Task<Result<bool, Errors>> Exists(Guid id, CancellationToken cancellationToken);
    Task<Result<bool, Errors>> AllExist(IEnumerable<Guid> ids, bool active, CancellationToken cancellationToken);
    Task<Result<Guid, Errors>> Delete(Location location);
}