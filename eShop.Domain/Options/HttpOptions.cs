namespace eShop.Domain.Options;

public class HttpOptions
{
    public required DataType Type { get; set; }
    public bool WithBearer { get; set; } = true;
    public bool ValidateToken { get; set; } = true;
}