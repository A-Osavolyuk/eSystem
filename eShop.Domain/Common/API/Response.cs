using Newtonsoft.Json;

namespace eShop.Domain.Common.API;

public class Response
{
    public string Message { get; set; } = string.Empty;
    public object? Result { get; set; }
    public bool Success { get; set; }

    public static Response Create(string message, object? result = null, bool isSucceeded = false)
    {
        return new Response
        {
            Message = message,
            Result = result,
            Success = isSucceeded
        };
    }
    
    public TValue? Get<TValue>()
    {
        var json = JsonConvert.SerializeObject(Result);
        var value = JsonConvert.DeserializeObject<TValue>(json);
        return value;
    }
}