using System.Text.Json;

namespace eSecurity.Client.Common.JS.Fetch;

public sealed class JsResult
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public JsError? Error { get; set; }
    
    public T Get<T>()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        var value = JsonSerializer.Deserialize<T>(Data!.ToString()!, options)!;
        return value;
    }
}

public sealed class JsError
{
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}