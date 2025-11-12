using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSystem.Core.Data;

public static class ModelBuilderExtensions
{
    extension<TProperty>(PropertyBuilder<TProperty> builder) where TProperty : struct
    {
        public PropertyBuilder<TProperty> HasEnumConversion()
        {
            return builder.HasConversion(
                value => value.ToString(),
                value => Enum.Parse<TProperty>(value!));
        }
    }
}
