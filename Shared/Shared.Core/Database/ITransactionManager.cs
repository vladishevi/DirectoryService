using CSharpFunctionalExtensions;

namespace Shared.Core.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Errors.Errors>> BeginAsync(CancellationToken cancellationToken);
    Task<UnitResult<Errors.Errors>> SaveChangesAsync(CancellationToken cancellationToken);
}