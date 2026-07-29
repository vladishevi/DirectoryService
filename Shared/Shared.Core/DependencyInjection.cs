using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Abstractions;

namespace Shared.Core;

public static class DependencyInjection
{
    public static void AddHandlersFromAssembly(this IServiceCollection services) =>
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes
                .AssignableToAny(
                    typeof(ICommandHandler<,>),
                    typeof(ICommandHandler<>),
                    typeof(IQueryHandler<,>),
                    typeof(IQueryHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
}
