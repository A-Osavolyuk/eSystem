namespace eSecurity.Core.Common.Requests;

public class AuthorizeRequest
{
    public required Guid UserId { get; set; }
    public required string ResponseType { get; set; }
    public required string ClientId { get; set; }
    public required string RedirectUri { get; set; }
    public required List<string> Scopes { get; set; }
    public required string Nonce { get; set; }
    public required string State { get; set; }
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
}