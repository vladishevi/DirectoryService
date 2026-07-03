using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Features.Departments;
using DirectoryService.Application.Features.Locations;
using DirectoryService.Application.Features.Positions;
using DirectoryService.Infrastructure.Postgres.BackgroundServices;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Infrastructure.Postgres.Transaction;
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
        AddTransactionExceptionHandlers(services);
        AddBackgroundServices(services);
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        return services;
    }

    private static void AddRepositories(IServiceCollection services) =>
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes
                .AssignableToAny(typeof(IRepository)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
    
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

        services.AddScoped<IReadDbContext>(s => s.GetRequiredService<DirectoryServiceDbContext>());
    }

    private static void AddTransactionExceptionHandlers(IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes
                .AssignableToAny(typeof(ITransactionExceptionHandler)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
    }

    private static void AddBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<CleanupBackgroundService>();
        services.AddScoped<CleanupService>();
    }
}
