using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Netcorext.Extensions.AspNetCore.ModelBinding;

namespace Microsoft.Extensions.DependencyInjection;

public static class MvcOptionsExtensions
{
    public static MvcOptions UseRoutePrefix(this MvcOptions options, string prefix = "api/v{version:apiVersion}")
    {
        options.Conventions.Add(new RoutePrefixConvention(prefix));

        return options;
    }

    public static MvcOptions AddFromFormOrBodyBinderProvider(this MvcOptions options)
    {
        var providers = new IModelBinderProvider[]
                        {
                            options.ModelBinderProviders.OfType<BodyModelBinderProvider>().Single(),
                            options.ModelBinderProviders.OfType<ComplexObjectModelBinderProvider>().Single(),
                            options.ModelBinderProviders.OfType<SimpleTypeModelBinderProvider>().Single()
                        };
        
        options.ModelBinderProviders.Insert(0, new FromFormOrBodyBinderProvider(providers));
        
        return options;
    }
}