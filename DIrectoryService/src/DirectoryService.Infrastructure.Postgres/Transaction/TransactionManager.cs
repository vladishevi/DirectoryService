using System.Data.Common;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared.Core.Database;
using Shared.Errors;

namespace DirectoryService.Infrastructure.Postgres.Transaction;

public class TransactionManager : ITransactionManager
{
    private readonly DbContext _dbContext;
    private readonly IEnumerable<ITransactionExceptionHandler> _transactionExceptionHandlers;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public TransactionManager(
        DirectoryServiceDbContext dbContext,
        IEnumerable<ITransactionExceptionHandler> transactionExceptionHandlers, 
        ILogger<TransactionManager> logger, 
        ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _transactionExceptionHandlers = transactionExceptionHandlers;
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
        catch (Exception exp)
        {
            foreach (ITransactionExceptionHandler handler in _transactionExceptionHandlers)
            {
                if (handler.TryHandle(exp, out UnitResult<Errors> result))
                {
                    return result;
                }
            }
        
            switch (exp)
            {
                case DbException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } }:
                    return Error.Conflict().ToErrors();
                case DbException { InnerException: PostgresException }:
                    _logger.LogError("Database exception while processing transaction");
                    return GeneralErrors.DatabaseError().ToErrors();
                case OperationCanceledException:
                    _logger.LogWarning("Operation cancelled while processing transaction");
                    return GeneralErrors.OperationCancelled().ToErrors();
                default:
                    _logger.LogError(exp, "Database error while processing transaction");
                    return GeneralErrors.DatabaseError().ToErrors();
            }
        }
    }
}