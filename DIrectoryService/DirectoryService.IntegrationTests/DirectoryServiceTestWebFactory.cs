using DirectoryService.Application.Database;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace DirectoryService.IntegrationTests;

public class DirectoryServiceTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithDatabase("directory_service_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DirectoryServiceDbContext>();

            services.AddDbContext<DirectoryServiceDbContext>((sp, options) =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());

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
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
    }
}