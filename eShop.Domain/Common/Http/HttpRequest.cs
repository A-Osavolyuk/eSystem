namespace eShop.Domain.Common.Http;

public class HttpRequest
{
    public required string Url { get; set; }
    public required HttpMethod Method { get; set; } = HttpMethod.Get;
    public object? Data { get; set; }
    public Metadata? Metadata { get; set; }
}

public class Metadata
{
    public string Identifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}