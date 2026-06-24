using eSystem.Core.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eSecurity.Idp.Common.Binding;

public sealed class EnumModelBinder<TEnum> : IModelBinder where TEnum : struct, Enum
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var stringValue = context.ValueProvider
            .GetValue(context.ModelName)
            .FirstValue;

        if (string.IsNullOrWhiteSpace(stringValue))
            return Task.CompletedTask;

        if (!EnumHelper.TryParseFromString<TEnum>(stringValue, out var enumValue))
        {
            context.ModelState.AddModelError(context.ModelName, $"'{context.ModelName}' is invalid");
            return Task.CompletedTask;
        }
        
        context.Result = ModelBindingResult.Success(enumValue.Value);
        return Task.CompletedTask;
    }
}