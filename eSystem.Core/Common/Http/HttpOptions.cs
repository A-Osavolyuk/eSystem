namespace eSystem.Core.Common.Http;

public sealed class HttpOptions
{
    public required DataType Type { get; set; }
    public bool WithBearer { get; set; }
}