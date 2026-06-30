using CSharpFunctionalExtensions;
using Shared;
using Shared.Errors;

namespace DirectoryService.Application.Database;

public interface ITransactionScope : IDisposable
{
    Task<UnitResult<Errors>> Commit(CancellationToken cancellationToken);
    Task<UnitResult<Errors>> Rollback(CancellationToken cancellationToken);
}