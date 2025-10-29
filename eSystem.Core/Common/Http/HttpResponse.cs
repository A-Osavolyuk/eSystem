using System.Text.Json;

namespace eSystem.Core.Common.Http;

public sealed class HttpResponse
{
    public string Message { get; set; } = string.Empty;
    public object? Result { get; set; }
    public bool Success { get; set; }

    public static HttpResponse Create(string? message, object? result = null, bool isSucceeded = false)
    {
        return new HttpResponse
        {
            Message = message ?? string.Empty,
            Result = result,
            Success = isSucceeded
        };
    }
    
    public TValue? Get<TValue>()
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true, 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        var json = JsonSerializer.Serialize(Result, options);
        var value = JsonSerializer.Deserialize<TValue>(json, options);
        return value;
    }
}