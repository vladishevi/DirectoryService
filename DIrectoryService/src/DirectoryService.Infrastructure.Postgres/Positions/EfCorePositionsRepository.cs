using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Positions;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared;

namespace DirectoryService.Infrastructure.Postgres;

public class EfCorePositionsRepository : IPositionsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<EfCorePositionsRepository> _logger;

    public EfCorePositionsRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<EfCorePositionsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Add(Position position, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Positions.AddAsync(position, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return position.Id;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException pgException)
        {
            if (pgException.SqlState != PostgresErrorCodes.UniqueViolation)
            {
                _logger.LogError("Database update error while creating new department with name {name}",
                    position.Name.Value);
                return PositionsErrors.DatabaseError().ToErrors();
            }

            if (pgException.ConstraintName.Contains(Constants.Indexes.POSITION_NAME,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return PositionsErrors.NameConflict(position.Name.Value).ToErrors();
            }
            
            return Error.Conflict().ToErrors();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while creating new position with name {name}",
                position.Name.Value);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while creating new position with name {name}",
                position.Name.Value);
            return GeneralErrors.DatabaseError().ToErrors();
        }
    }
}