using eSystem.Core.Http.Results;

namespace eSystem.Core.Http;

public sealed class HttpResponse
{
    public bool Succeeded { get; init; }
    private Error? Error { get; init; }
    public static HttpResponse Success() 
        => new(){ Succeeded = true };
    public static HttpResponse<TResponse> Success<TResponse>(TResponse response) 
        => new() { Succeeded = true, Result = response };
    public static HttpResponse Fail(Error error) 
        => new(){ Succeeded = false, Error = error };
    public static HttpResponse<TResponse> Fail<TResponse>(Error error) 
        => new(){ Succeeded = false, Error = error };
    
    public Error GetError() => Error!;

    public void Match(Action success, Action<Error> fail)
    {
        if (Succeeded) success();
        else fail(Error!);
    }
}

public sealed class HttpResponse<TResponse>
{
    public bool Succeeded { get; init; }
    public Error? Error { private get; init; }
    public TResponse? Result { private get; init; }
    
    public TResponse Get() => Result!;
    
    public Error GetError() => Error!;
    
    public void Match(Action<TResponse> success, Action<Error> fail)
    {
        if (Succeeded) success(Result!);
        else fail(Error!);
    }
    
    public async Task MatchAsync(Func<TResponse, Task> success, Action<Error> fail)
    {
        if (Succeeded) await success(Result!);
        else fail(Error!);
    }
    
    public async Task MatchAsync(Func<TResponse, Task> success, Func<Error, Task> fail)
    {
        if (Succeeded) await success(Result!);
        else await fail(Error!);
    }
}