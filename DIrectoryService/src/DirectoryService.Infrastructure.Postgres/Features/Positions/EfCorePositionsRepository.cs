using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Positions;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared;
using Shared.Errors;

namespace DirectoryService.Infrastructure.Postgres.Features.Positions;

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

    public async Task<Result<Position, Errors>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            Position? position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (position == null)
            {
                return GeneralErrors.NotFound("Position not found", id).ToErrors();
            }

            return position;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while getting position with id {id}", id);
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while getting position with id {id}", id);
            return GeneralErrors.DatabaseError().ToErrors();       
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

            IQueryable<Position> query = _dbContext.Positions.Where(p => ids.Contains(p.Id));
            if (active)
                query = query.Where(p => p.IsActive);

            int existingCount = await query
                .CountAsync(cancellationToken: cancellationToken);

            return existingCount == ids.Count();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while checking if positions exist");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while checking if positions exist");
            return PositionsErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<Result<Guid, Errors>> Delete(Position position)
    {
        try
        {
            _dbContext.Remove(position);
            return position.Id;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while deleting position with id {id}", position.Id);
            return GeneralErrors.DatabaseError().ToErrors();
        }
    }
}
