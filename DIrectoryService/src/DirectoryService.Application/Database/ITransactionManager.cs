using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Application.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Errors>> BeginAsync(CancellationToken cancellationToken);
    Task<UnitResult<Errors>> SaveChangesAsync(CancellationToken cancellationToken);
}