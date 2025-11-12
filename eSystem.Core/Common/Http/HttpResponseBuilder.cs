namespace eSystem.Core.Common.Http;

public sealed class HttpResponseBuilder
{
    private string? _message;
    private object? _result;
    private bool _isSucceeded;

    private HttpResponseBuilder(){}

    public static HttpResponseBuilder Create() => new HttpResponseBuilder();

    public HttpResponseBuilder WithResult(object? value)
    {
        _result = value;
        return this;
    }

    public HttpResponseBuilder WithMessage(string value)
    {
        _message = value;
        return this;
    }

    public HttpResponseBuilder Failed()
    {
        _isSucceeded = false;
        return this;
    }

    public HttpResponseBuilder Succeeded()
    {
        _isSucceeded = true;
        return this;
    }

    public HttpResponse Build()
    {
        return HttpResponse.Create(_message, _result, _isSucceeded);
    }
}