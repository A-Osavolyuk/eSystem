using eSystem.Core.Common.Http.Context;

namespace eSystem.Core.Common.Http;

public sealed class HttpOptions
{
    public required DataType Type { get; set; }
    public required AuthenticationType Authentication { get; set; }
}