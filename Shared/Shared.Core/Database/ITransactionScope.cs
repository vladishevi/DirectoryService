using CSharpFunctionalExtensions;

namespace Shared.Core.Database;

public interface ITransactionScope : IDisposable
{
    Task<UnitResult<Errors.Errors>> Commit(CancellationToken cancellationToken);
    Task<UnitResult<Errors.Errors>> Rollback(CancellationToken cancellationToken);
}