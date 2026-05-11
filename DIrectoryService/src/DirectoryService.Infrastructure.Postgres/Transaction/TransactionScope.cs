using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Transaction;

public class TransactionScope : ITransactionScope
{
    private readonly IDbContextTransaction _transaction;
    private readonly ILogger<TransactionScope> _logger;

    public TransactionScope(IDbContextTransaction transaction, ILogger<TransactionScope> logger)
    {
        _transaction = transaction;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Commit(CancellationToken cancellationToken)
    {
        try
        {
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Transaction committed");
            return UnitResult.Success<Errors>();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while committing transaction");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception)
        {
            _logger.LogError("Database error while committing transaction");
            return GeneralErrors.DatabaseError().ToErrors();
        }
    }

    public async Task<UnitResult<Errors>> Rollback(CancellationToken cancellationToken)
    {
        try
        {
            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogInformation("Transaction rolled back");
            return UnitResult.Success<Errors>();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while rolling back transaction");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception)
        {
            _logger.LogError("Database error while rolling back transaction");
            return GeneralErrors.DatabaseError().ToErrors();
        }
    }

    public void Dispose() => _transaction.Dispose();
}