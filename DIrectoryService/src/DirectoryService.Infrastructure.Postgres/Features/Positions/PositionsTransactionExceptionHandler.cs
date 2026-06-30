using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Positions;
using DirectoryService.Infrastructure.Postgres.Transaction;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared;
using Shared.Errors;

namespace DirectoryService.Infrastructure.Postgres.Features.Positions;

public class PositionsTransactionExceptionHandler : ITransactionExceptionHandler
{
    public bool TryHandle(Exception exp, out UnitResult<Errors> result)
    {
        if (exp is DbUpdateException { InnerException: PostgresException pgException })
        {
            if (pgException.ConstraintName.Contains(Constants.Indexes.POSITION_NAME,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                result = PositionsErrors.NameConflict().ToErrors();
                return true;
            }
        }
        result = default;
        return false;
    }
}