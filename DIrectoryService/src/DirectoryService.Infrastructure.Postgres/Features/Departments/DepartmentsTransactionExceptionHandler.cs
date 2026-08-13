using CSharpFunctionalExtensions;
using DirectoryService.Application.Features.Departments;
using DirectoryService.Infrastructure.Postgres.Transaction;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Errors;

namespace DirectoryService.Infrastructure.Postgres.Features.Departments;

public class DepartmentsTransactionExceptionHandler : ITransactionExceptionHandler
{
    public bool TryHandle(Exception exp, out UnitResult<Errors> result)
    {
        if (exp is DbUpdateException { InnerException: PostgresException pgException })
        {
            if (pgException.ConstraintName.Contains(Constants.Indexes.DEPARTMENT_NAME, StringComparison.InvariantCultureIgnoreCase))
            {
                result = DepartmentsErrors.NameConflict().ToErrors();
                return true;
            }
            if (pgException.ConstraintName.Contains(Constants.Indexes.DEPARTMENT_IDENTIFIER,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                result = DepartmentsErrors.IdentifierConflict().ToErrors();
                return true;
            }
        }
        result = default;
        return false;
    }
}