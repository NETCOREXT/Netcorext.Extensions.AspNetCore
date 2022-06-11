using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Netcorext.Extensions.AspNetCore.ModelBinding;

public class ArrayBinder : IModelBinder
{
    private readonly IEnumerable<IModelBinder> _modelBinders;
    private const string SEPARATOR = ",";

    public ArrayBinder(IEnumerable<IModelBinder> modelBinders)
    {
        _modelBinders = modelBinders;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        await SelfBindModelAsync(bindingContext);

        if (bindingContext.Result.IsModelSet) return;

        foreach (var modelBinder in _modelBinders)
        {
            bindingContext.ModelState.Clear();

            await modelBinder.BindModelAsync(bindingContext);

            if (bindingContext.Result.IsModelSet) return;
        }
    }

    private static Task SelfBindModelAsync(ModelBindingContext bindingContext)
    {
        var modelName = string.IsNullOrWhiteSpace(bindingContext.ModelName) ? bindingContext.FieldName : bindingContext.ModelName;
        var valueResult = bindingContext.ValueProvider.GetValue(modelName);
        var value = valueResult.Values.ToString();

        if (bindingContext.ModelMetadata is not DefaultModelMetadata metadata)
            return Task.CompletedTask;

        if (metadata.ElementType == null)
            return Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(value))
            return Task.CompletedTask;

        var values = value.Split(SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        var convert = TypeDescriptor.GetConverter(metadata.ElementType);

        if (!values.All(t => convert.IsValid(t)))
            return Task.CompletedTask;

        var model = values.Select(t => convert.ConvertFromString(t));

        var result = metadata.ElementType.Name switch
                     {
                         nameof(Boolean) => ModelBindingResult.Success(model.Cast<bool>().ToArray()),
                         nameof(Byte) => ModelBindingResult.Success(model.Cast<byte>().ToArray()),
                         nameof(Char) => ModelBindingResult.Success(model.Cast<char>().ToArray()),
                         nameof(DateTime) => ModelBindingResult.Success(model.Cast<DateTime>().ToArray()),
                         nameof(DateTimeOffset) => ModelBindingResult.Success(model.Cast<DateTimeOffset>().ToArray()),
                         nameof(Decimal) => ModelBindingResult.Success(model.Cast<decimal>().ToArray()),
                         nameof(Double) => ModelBindingResult.Success(model.Cast<double>().ToArray()),
                         nameof(Int16) => ModelBindingResult.Success(model.Cast<short>().ToArray()),
                         nameof(Int32) => ModelBindingResult.Success(model.Cast<int>().ToArray()),
                         nameof(Int64) => ModelBindingResult.Success(model.Cast<long>().ToArray()),
                         nameof(Object) => ModelBindingResult.Success(model.ToArray()),
                         nameof(SByte) => ModelBindingResult.Success(model.Cast<sbyte>().ToArray()),
                         nameof(Single) => ModelBindingResult.Success(model.Cast<float>().ToArray()),
                         nameof(String) => ModelBindingResult.Success(model.Cast<string>().ToArray()),
                         nameof(TimeSpan) => ModelBindingResult.Success(model.Cast<TimeSpan>().ToArray()),
                         nameof(UInt16) => ModelBindingResult.Success(model.Cast<ushort>().ToArray()),
                         nameof(UInt32) => ModelBindingResult.Success(model.Cast<uint>().ToArray()),
                         nameof(UInt64) => ModelBindingResult.Success(model.Cast<ulong>().ToArray()),
                         _ => default
                     };

        if (result.IsModelSet)
            bindingContext.Result = result;

        return Task.CompletedTask;
    }
}