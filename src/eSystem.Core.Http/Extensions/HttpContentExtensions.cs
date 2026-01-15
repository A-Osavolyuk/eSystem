using System.Text.Json;

namespace eSystem.Core.Http.Extensions;

public static class HttpContentExtensions
{
    extension(HttpContent content)
    {
        public async Task<T> ReadAsync<T>(CancellationToken cancellationToken = default)
        {
            var json = await content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(json)!;
        }

        public async Task<T> ReadAsync<T>(JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            var json = await content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(json, options)!;
        }
    }
}