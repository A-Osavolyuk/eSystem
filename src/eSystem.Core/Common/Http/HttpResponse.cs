using System.Text.Json;
using eSystem.Core.Common.Results;

namespace eSystem.Core.Common.Http;

public sealed class HttpResponse
{
    private Error? Error { get; set; }
    public bool Succeeded { get; set; }
    public static HttpResponse Success() => new(){ Succeeded = true };
    public static HttpResponse Fail(Error error) => new(){ Succeeded = false, Error = error };
    public Error GetError() => Error!;
}

public sealed class HttpResponse<TResponse>
{
    private Error? Error { get; set; }
    public bool Succeeded { get; set; }
    private TResponse? Result { get; set; }

    public static HttpResponse<TResponse> Success(TResponse response) => new()
    {
        Succeeded = true,
        Result = response
    };
    
    public static HttpResponse<TResponse> Fail(Error error) => new()
    {
        Succeeded = false,
        Error = error
    };
    
    public TResponse Get()
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true, 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        var json = JsonSerializer.Serialize(Result, options);
        var value = JsonSerializer.Deserialize<TResponse>(json, options)!;
        return value;
    }
    
    public Error GetError() => Error!;
}