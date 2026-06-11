using DirectoryService.Presenters.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Exceptions;

namespace DirectoryService.Presenters;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHttpLogging();

        return services;
    }

    public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((services, lc) =>
        {
            lc.ReadFrom.Configuration(configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Application", "Directory Service");
        });
        return services;
    }

    public static WebApplication Configure(this WebApplication app)
    {
        app.UseExceptionMiddleware();
        app.UseSerilogRequestLogging();
        //app.UseHttpLogging();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Directory Service"));
        }

        app.MapControllers();

        return app;
    }
}
