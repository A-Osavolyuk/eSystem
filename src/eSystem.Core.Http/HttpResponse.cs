using System.Text.Json;
using eSystem.Core.Http.Results;

namespace eSystem.Core.Http;

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
    
    public TResponse Get() => (TResponse)Result!;
    
    public Error GetError() => Error!;
}