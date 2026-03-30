using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static void AddPostgresInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDb(services, configuration);
    }

    private static void AddDb(IServiceCollection services, IConfiguration configuration) =>
        services.AddScoped<DirectoryServiceDbContext>(_ =>
            new DirectoryServiceDbContext(configuration.GetConnectionString("DirectoryServiceDb")));
} 