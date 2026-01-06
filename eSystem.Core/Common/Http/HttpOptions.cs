namespace eSystem.Core.Common.Http;

public sealed class HttpOptions
{
    public required AuthenticationType Authentication { get; set; }
    public required string ContentType { get; set; }
    
    public bool WithLocale { get; set; }
    public bool WithTimezone { get; set; }
}