using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace eSecurity.Server.Data;

public static class ValueConverters
{
    public static ValueConverter<TimeSpan?, long?> NullableTimeSpan { get; } = new(
        v => v.HasValue ? v.Value.Ticks : null,
        v => v.HasValue ? TimeSpan.FromTicks(v.Value) : null
    );
}