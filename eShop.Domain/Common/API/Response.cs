using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        var json = JsonConvert.SerializeObject(Result,  Formatting.Indented, new JsonSerializerSettings()
            {
                Converters = [ new StringEnumConverter() ]
            });
        var value = JsonConvert.DeserializeObject<TValue>(json);
        return value;
    }
}