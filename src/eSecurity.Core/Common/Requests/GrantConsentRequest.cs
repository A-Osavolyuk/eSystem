namespace eSecurity.Core.Common.Requests;

public class GrantConsentRequest
{
    public required Guid UserId { get; set; }
    public required Guid ClientId { get; set; }
    public required List<string> Scopes { get; set; }
}