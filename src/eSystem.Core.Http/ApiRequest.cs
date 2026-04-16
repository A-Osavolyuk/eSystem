namespace eSystem.Core.Http;

public sealed class ApiRequest
{
    public required string Url { get; set; }
    public required HttpMethods Method { get; set; }
    public object? Data { get; set; }
}
