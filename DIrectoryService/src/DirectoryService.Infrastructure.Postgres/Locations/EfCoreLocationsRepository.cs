using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Locations;

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
            await _dbContext.SaveChangesAsync(cancellationToken);
            return location.Id;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException pgException)
        {
            if (pgException.SqlState != PostgresErrorCodes.UniqueViolation)
            {
                _logger.LogError("Database update error while creating new location with name {name}",
                    location.Name.Value);
                
                return LocationsErrors.DatabaseError().ToErrors();
            }

            if (pgException.ConstraintName.Contains(nameof(Location.Name),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return LocationsErrors.NameConflict(location.Name.Value).ToErrors();
            }
                
            if (pgException.ConstraintName.Contains(Constants.Indexes.LOCATION_ADDRESS, StringComparison.InvariantCultureIgnoreCase))
            {
                return LocationsErrors.AddressConflict(location.Address.ToString()).ToErrors();
            }

            return Error.Conflict().ToErrors();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while creating new location with name {name}", location.Name.Value);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Database error while creating new location with name {name}", location.Name.Value);
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
}