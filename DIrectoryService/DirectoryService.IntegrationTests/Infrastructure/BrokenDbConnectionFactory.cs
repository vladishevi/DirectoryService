using System.Data;
using DirectoryService.Application.Database;

namespace DirectoryService.IntegrationTests.Infrastructure;

public sealed class BrokenDbConnectionFactory : IDbConnectionFactory
{
    public Task<IDbConnection> CreateConnectionAsync(CancellationToken ct)
    {
        throw new InvalidOperationException("Test database connection failure.");
    }
}
