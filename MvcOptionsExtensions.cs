using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection;

public static class MvcOptionsExtensions
{
    public static MvcOptions UseRoutePrefix(this MvcOptions options, string prefix = "api/v{version:apiVersion}")
    {
        options.Conventions.Add(new RoutePrefixConvention(prefix));

        return options;
    }
}