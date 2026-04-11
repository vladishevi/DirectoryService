using DirectoryService.Application.Locations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<LocationsService>();
        
        return services;
    }
}