using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSystem.Core.Data;

public static class ModelBuilderExtensions
{
    public static PropertyBuilder<TProperty> HasEnumConversion<TProperty>(this PropertyBuilder<TProperty> builder)
        where TProperty : struct
    {
        return builder.HasConversion(
            value => value.ToString(),
            value => Enum.Parse<TProperty>(value!));
    }
}
