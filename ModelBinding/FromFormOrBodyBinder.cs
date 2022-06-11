using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Netcorext.Extensions.AspNetCore.ModelBinding;

public class FromFormOrBodyBinder : IModelBinder
{
    private readonly IEnumerable<IModelBinder> _modelBinders;

    public FromFormOrBodyBinder(IEnumerable<IModelBinder> modelBinders)
    {
        _modelBinders = modelBinders;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        foreach (var modelBinder in _modelBinders)
        {
            bindingContext.ModelState.Clear();

            await modelBinder.BindModelAsync(bindingContext);

            if (bindingContext.Result.IsModelSet) return;
        }
    }
}