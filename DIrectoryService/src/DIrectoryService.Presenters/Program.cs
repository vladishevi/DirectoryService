using System.Globalization;
using DirectoryService.Application;
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
        .AddPostgresInfrastructure(builder.Configuration)
        .AddApplicationServices();
    
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

namespace DirectoryService.Presenters
{
    public partial class Program;
}