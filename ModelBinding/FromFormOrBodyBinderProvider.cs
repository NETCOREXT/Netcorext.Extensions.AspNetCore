using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Netcorext.Extensions.AspNetCore.ModelBinding;

public class FromFormOrBodyBinderProvider : IModelBinderProvider
{
    private readonly IEnumerable<IModelBinderProvider> _modelBinderProviders;

    public FromFormOrBodyBinderProvider(IEnumerable<IModelBinderProvider> modelBinderProviders)
    {
        _modelBinderProviders = modelBinderProviders;
    }
    
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.BindingInfo.BindingSource == null ||
            (!context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Body)
          && !context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Form))
             ) return null;

        var binders = _modelBinderProviders.Select(t => t.GetBinder(context))
                                           .Where(t => t != null);
        
        return new FromFormOrBodyBinder(binders!);
    }
}