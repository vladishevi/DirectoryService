using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Errors;

namespace DirectoryService.Infrastructure.Postgres.Features.Locations;

public class EfCoreLocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<EfCoreLocationsRepository> _logger;

    public EfCoreLocationsRepository(DirectoryServiceDbContext dbContext, ILogger<EfCoreLocationsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Add(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Locations.AddAsync(location, cancellationToken);
            return location.Id;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while adding new location with name {name}", location.Name.Value);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Database error while adding new location with name {name}", location.Name.Value);
            return LocationsErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<Result<Location, Errors>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Location? location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
            if (location != null)
            {
                return location;
            }

            _logger.LogWarning("Location not found with id {id}", id);
            return GeneralErrors.NotFound("Location not found", id).ToErrors();

        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while getting location with id {id}", id);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Database error while getting location with id {id}", id);
            return LocationsErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<Result<bool, Errors>> Exists(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Locations.AnyAsync(l => l.Id == id, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while checking if location with id {id} exists", id);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Database error while checking if location with id {id} exists", id);
            return LocationsErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<Result<bool, Errors>> AllExist(IEnumerable<Guid> ids, bool active, CancellationToken cancellationToken)
    {
        try
        {
            if (ids.Distinct().Count() != ids.Count())
            {
                return GeneralErrors.Dublicate(message: "Ids must be unique").ToErrors();
            }

            IQueryable<Location> query = _dbContext.Locations.Where(l => ids.Contains(l.Id));
            if (active)
                query = query.Where(l => !l.IsDeleted);

            int existingCount = await query
                .CountAsync(cancellationToken: cancellationToken);

            return existingCount == ids.Count();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while checking if locations exist");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Database error while checking if locations exist");
            return LocationsErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<Result<Guid, Errors>> Delete(Location location)
    {
        try
        {
            _dbContext.Remove(location);
            return location.Id;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while deleting location with id {id}", location.Id);
            return GeneralErrors.DatabaseError().ToErrors();
        }
    }
}