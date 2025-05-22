namespace eShop.Domain.Options;

public class HttpOptions
{
    public bool WithBearer { get; set; } = true;
    public bool ValidateToken { get; set; } = true;
}