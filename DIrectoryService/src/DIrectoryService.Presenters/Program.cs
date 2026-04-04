using System.Globalization;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Presenters;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateLogger();

try
{
    Log.Information("Starting Directory Service");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services
        .AddSerilogLogging(builder.Configuration)
        .AddApiServices()
        .AddPostgresInfrastructure(builder.Configuration);
    
    WebApplication app = builder.Build();

    app.Configure();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
