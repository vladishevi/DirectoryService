using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Infrastructure.Postgres.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Features.Locations;

public class LocationsTransactionExceptionHandler : ITransactionExceptionHandler
{
    private readonly ILogger<LocationsTransactionExceptionHandler> _logger;

    public LocationsTransactionExceptionHandler(ILogger<LocationsTransactionExceptionHandler> logger)
    {
        _logger = logger;
    }

    public bool TryHandle(Exception exp, out UnitResult<Errors> result)
    {
        if (exp is DbUpdateException { InnerException: PostgresException pgException })
        {
            if (pgException.ConstraintName.Contains(nameof(Location.Name),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                result = LocationsErrors.NameConflict().ToErrors();
                return true;
            }
                
            if (pgException.ConstraintName.Contains(Constants.Indexes.LOCATION_ADDRESS, StringComparison.InvariantCultureIgnoreCase))
            {
                result = LocationsErrors.AddressConflict().ToErrors();
                return true;
            }
        }

        result = null;
        return false;
    }
}