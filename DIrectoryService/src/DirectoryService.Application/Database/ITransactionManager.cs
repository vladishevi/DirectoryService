using CSharpFunctionalExtensions;
using DirectoryService.Infrastructure.Postgres.Transaction;
using Shared;

namespace DirectoryService.Application.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Errors>> BeginAsync(CancellationToken cancellationToken);
    Task<Result<T,Errors>> SaveChangesAsync<T>(TransactionExceptionHandler<T> exceptionHandler, CancellationToken cancellationToken);
    Task<UnitResult<Errors> SaveChangesAsync<(TransactionExceptionHandler exceptionHandler, CancellationToken cancellationToken);
}