using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Transaction;

/// <summary>
/// Represents a handler for handling exceptions that occur during a transaction.
/// </summary>
public interface ITransactionExceptionHandler
{
    bool TryHandle(Exception exp, out UnitResult<Errors> result);
}