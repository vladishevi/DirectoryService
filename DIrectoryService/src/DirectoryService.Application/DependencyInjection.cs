using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ICommandHandler<Guid, CreateLocationCommand>, CreateLocationHandler>();
        
        return services;
    }
}