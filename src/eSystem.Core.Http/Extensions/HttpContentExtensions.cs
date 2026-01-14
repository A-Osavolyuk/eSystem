using System.Text.Json;

namespace eSystem.Core.Http.Extensions;

public static class HttpContentExtensions
{
    public static async Task<T> ReadAsync<T>(this HttpContent content, JsonSerializerOptions options)
    {
        var json = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, options)!;
    }
}