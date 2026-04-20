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
            if (pgException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                if (pgException.ConstraintName.Contains(Indexes.ADDRESS, StringComparison.InvariantCultureIgnoreCase))
                {
                    return LocationsErrors.AddressConflict(location.Address.ToString()).ToErrors();
                }

                if (pgException.ConstraintName.Contains(nameof(Location.Name),
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return LocationsErrors.NameConflict(location.Name.Value).ToErrors();
                }

                return Error.Conflict().ToErrors();
            }

            _logger.LogError("Database update error while creating new location with name {name}", location.Name.Value);
            return LocationsErrors.DatabaseError().ToErrors();
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
}