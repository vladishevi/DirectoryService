using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddHandlersFromAssembly(typeof(DependencyInjection).Assembly);
        
        return services;
    }
}
