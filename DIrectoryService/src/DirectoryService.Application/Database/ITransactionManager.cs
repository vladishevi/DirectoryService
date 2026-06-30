using CSharpFunctionalExtensions;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Errors>> BeginAsync(CancellationToken cancellationToken);
    Task<UnitResult<Errors>> SaveChangesAsync(CancellationToken cancellationToken);
}