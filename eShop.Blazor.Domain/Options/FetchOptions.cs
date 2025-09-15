namespace eShop.Blazor.Domain.Options;

public class FetchOptions
{
    public HttpMethod Method { get; set; } = HttpMethod.Post;
    public string Url { get; set; } = string.Empty;
    public string Credentials { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = [];
    public object? Body { get; set; }
}