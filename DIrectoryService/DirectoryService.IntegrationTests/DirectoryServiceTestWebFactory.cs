using System.Data.Common;
using DirectoryService.Application.Database;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace DirectoryService.IntegrationTests;

public class DirectoryServiceTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase("directory_service_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private Respawner _respawner;
    private DbConnection _dbConnection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DirectoryServiceDbContext>();

            services.AddDbContext<DirectoryServiceDbContext>((sp, options) =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
                options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

                ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                if (sp.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
                options.UseLoggerFactory(loggerFactory);
            });

            services.AddScoped<IReadDbContext>(s => s.GetRequiredService<DirectoryServiceDbContext>());
        });
    }
    
    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
        await dbContext.Database.EnsureCreatedAsync();

        await CreateDatabaseConnectionAsync();
        await InitializeRespawnerAsync();
    }

    private async Task CreateDatabaseConnectionAsync()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_postgresContainer.GetConnectionString());
        var dataSource = dataSourceBuilder.Build();
        _dbConnection = await dataSource.OpenConnectionAsync();
    }

    public async Task ResetDbAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    private async Task InitializeRespawnerAsync()
    {
        _respawner = await Respawner.CreateAsync(_dbConnection,
            new RespawnerOptions { DbAdapter = DbAdapter.Postgres, SchemasToInclude = ["public"] });
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbConnection.CloseAsync();
        await _dbConnection.DisposeAsync();
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
    }
}
