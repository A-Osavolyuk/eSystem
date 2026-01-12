namespace eSecurity.Core.Common.Requests;

public class CheckConsentRequest
{
    public required Guid ClientId { get; set; }
    public required Guid UserId { get; set; }
    public required List<string> Scopes { get; set; }
}