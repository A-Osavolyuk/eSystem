namespace eSecurity.Core.Common.Requests;

public class CheckConsentRequest
{
    public required Guid ClientId { get; set; }
    public required Guid SessionId { get; set; }
    public required List<string> Scopes { get; set; }
}