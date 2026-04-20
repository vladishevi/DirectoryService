using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Shared;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, Errors>> Add(Location location, CancellationToken cancellationToken);
    Task<Result<Location, Errors>> GetById(Guid id, CancellationToken cancellationToken);
}