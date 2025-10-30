using System.Text.Json;

namespace eAccount.Common.Http.Extensions;

public static class HttpContentExtensions
{
    public static async Task<T> ReadAsAsync<T>(this HttpContent content, JsonSerializerOptions options)
    {
        var json = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, options)!;
    }
}