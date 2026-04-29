using DirectoryService.Application.Features.Departments;
using DirectoryService.Application.Features.Locations;
using DirectoryService.Application.Features.Positions;
using DirectoryService.Infrastructure.Postgres.Departments;
using DirectoryService.Infrastructure.Postgres.Locations;
using DirectoryService.Infrastructure.Postgres.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDb(services, configuration);
        AddRepositories(services);
        return services;
    }

    private static void AddRepositories(IServiceCollection services) =>
        services
            .AddScoped<ILocationsRepository, EfCoreLocationsRepository>()
            .AddScoped<IDepartmentsRepository, EfCoreDepartmentsRepository>()
            .AddScoped<IPositionsRepository, EfCorePositionsRepository>();

    private static void AddDb(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DirectoryServiceDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString(Constants.DATABASE));

            ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            if (sp.GetRequiredService<IHostEnvironment>().IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
            options.UseLoggerFactory(loggerFactory);
        });
    }
}
