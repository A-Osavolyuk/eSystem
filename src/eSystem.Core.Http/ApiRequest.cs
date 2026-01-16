using eSystem.Core.Requests;

namespace eSystem.Core.Http;

public sealed class ApiRequest
{
    public required string Url { get; set; }
    public required HttpMethod Method { get; set; } = HttpMethod.Get;
    public object? Data { get; set; }
    public Metadata? Metadata { get; set; }
}
