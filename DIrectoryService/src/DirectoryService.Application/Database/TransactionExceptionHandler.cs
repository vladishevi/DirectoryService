using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Transaction;

public class TransactionExceptionHandler<T>
{
    private readonly ILogger<TransactionExceptionHandler<T>> _logger;

    public TransactionExceptionHandler(ILogger<TransactionExceptionHandler<T>> logger)
    {
        _logger = logger;
    }
    
    public Result<T, Errors> Handle(Exception exp)
    {
        Result<T, Errors>? result = HandleInternal(exp);

        if (result is not null)
            return (Result<T, Errors>)result;
        
        if (exp is OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while processing transaction");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        
        _logger.LogError(exp, "Database error while processing transaction");
        return GeneralErrors.DatabaseError().ToErrors();       
    }

    protected virtual Result<T, Errors>? HandleInternal(Exception exp)
    {
        return null;       
    }
}

public class TransactionExceptionHandler
{
    private readonly ILogger<TransactionExceptionHandler> _logger;

    public TransactionExceptionHandler(ILogger<TransactionExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public UnitResult<Errors> Handle(Exception exp)
    {
        UnitResult<Errors>? result = HandleInternal(exp);

        if (result is not null)
            return (UnitResult<Errors>)result;
        
        if (exp is OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled while processing transaction");
            return GeneralErrors.OperationCancelled().ToErrors();
        }
        
        _logger.LogError(exp, "Database error while processing transaction");
        return GeneralErrors.DatabaseError().ToErrors();       
    }

    protected virtual UnitResult<Errors>? HandleInternal(Exception exp)
    {
        return null;       
    }
}