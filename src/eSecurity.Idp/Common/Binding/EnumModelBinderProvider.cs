using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eSecurity.Idp.Common.Binding;

public sealed class EnumModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var modelType = context.Metadata.ModelType;
        var enumType = Nullable.GetUnderlyingType(modelType) ?? modelType;
        if (!enumType.IsEnum)
            return null;

        var binderType = typeof(EnumModelBinder<>).MakeGenericType(enumType);
        return (IModelBinder)Activator.CreateInstance(binderType)!;
    }
}