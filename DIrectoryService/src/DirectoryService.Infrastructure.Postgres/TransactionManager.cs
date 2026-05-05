using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres;

public class TransactionManager : ITransactionManager
{
    private readonly DbContext _dbContext;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public TransactionManager(DirectoryServiceDbContext dbContext, ILogger<TransactionManager> logger, ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }
    
    public async Task<Result<ITransactionScope, Errors>> BeginAsync(CancellationToken cancellationToken)
    {
        try
        {
            IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            ILogger<TransactionScope> transactionLogger = _loggerFactory.CreateLogger<TransactionScope>();
            TransactionScope transactionScope = new(transaction, transactionLogger);

            return transactionScope;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while starting transaction");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database error while starting transaction");
            return GeneralErrors.DatabaseError().ToErrors();
        }       
    }
    
    public async Task<UnitResult<Errors>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Changes saved");
            return UnitResult.Success<Errors>();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while saving changes");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        catch (Exception)
        {
            _logger.LogError("Database error while saving changes");
            return GeneralErrors.DatabaseError().ToErrors();       
        }
    }
}

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