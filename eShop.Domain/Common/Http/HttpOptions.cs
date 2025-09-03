namespace eShop.Domain.Common.Http;

public class HttpOptions
{
    public required DataType Type { get; set; }
    public bool WithBearer { get; set; } = true;
}