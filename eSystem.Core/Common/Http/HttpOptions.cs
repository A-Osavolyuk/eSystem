using eSystem.Core.Common.Http.Context;

namespace eSystem.Core.Common.Http;

public sealed class HttpOptions
{
    public required AuthenticationType Authentication { get; set; }
    public required string ContentType { get; set; }
}