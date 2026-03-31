using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static void AddPostgresInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDb(services, configuration);
    }

    private static void AddDb(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DirectoryServiceDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DirectoryServiceDb")));
    }
} 