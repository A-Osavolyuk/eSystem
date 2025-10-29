namespace eSystem.Core.Common.Http;

public sealed class HttpResponseBuilder
{
    private string? message;
    private object? result;
    private bool isSucceeded;

    private HttpResponseBuilder(){}

    public static HttpResponseBuilder Create() => new HttpResponseBuilder();

    public HttpResponseBuilder WithResult(object? value)
    {
        result = value;
        return this;
    }

    public HttpResponseBuilder WithMessage(string value)
    {
        message = value;
        return this;
    }

    public HttpResponseBuilder Failed()
    {
        isSucceeded = false;
        return this;
    }

    public HttpResponseBuilder Succeeded()
    {
        isSucceeded = true;
        return this;
    }

    public HttpResponse Build()
    {
        return HttpResponse.Create(message, result, isSucceeded);
    }
}