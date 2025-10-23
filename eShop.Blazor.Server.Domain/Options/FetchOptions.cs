namespace eShop.Blazor.Server.Domain.Options;

public class FetchOptions
{
    public HttpMethod Method { get; set; } = HttpMethod.Post;
    public string Url { get; set; } = string.Empty;
    public object? Body { get; set; }
}