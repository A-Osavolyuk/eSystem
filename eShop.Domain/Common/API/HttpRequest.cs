namespace eShop.Domain.Common.API;

public class HttpRequest
{
    public required string Url { get; set; }
    public required HttpMethod Method { get; set; } = HttpMethod.Get;
    public object? Data { get; set; }
}