using Microsoft.Extensions.DependencyInjection.Extensions;
using Netcorext.Contracts;
using Netcorext.Extensions.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddContextState(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.TryAddScoped<IContextState, ContextState>();

        return services;
    }
}
