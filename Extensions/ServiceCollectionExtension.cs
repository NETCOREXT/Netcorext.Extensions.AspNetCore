using Microsoft.Extensions.DependencyInjection.Extensions;
using Netcorext.Contracts;
using Netcorext.Extensions.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddContextState(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        services.AddHttpContextAccessor();

        services.TryAdd(new ServiceDescriptor(typeof(IContextState), typeof(ContextState), lifetime));

        return services;
    }
}
