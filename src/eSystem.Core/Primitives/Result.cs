using System.Net;

namespace eSystem.Core.Primitives;

public class Result
{
    public required HttpStatusCode StatusCode { get; init; }
    public required bool Succeeded { get; init; }
    public Error? Error { get; init; }

    public Error GetError()
    {
        if (Error is null)
            throw new NullReferenceException("Error is null");

        return Error;
    }

    public TResponse Match<TResponse>(Func<Result, TResponse> success, Func<Result, TResponse> failure)
        => Succeeded ? success(this) : failure(this);
}