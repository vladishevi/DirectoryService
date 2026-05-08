using CSharpFunctionalExtensions;
using DirectoryService.Infrastructure.Postgres.Transaction;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Features.Departments;

public class LocationsTransactionExceptionHandler : TransactionExceptionHandler
{
    private readonly ILogger<TransactionExceptionHandler> _logger;

    public LocationsTransactionExceptionHandler(ILogger<TransactionExceptionHandler> logger) : base(logger)
    {
        _logger = logger;
    }

    protected override UnitResult<Errors>? HandleInternal(Exception exp)
    {
        
    }
}