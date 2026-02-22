using eSecurity.Core.Security.Credentials.PublicKey;

namespace eSecurity.Core.Common.Requests;

public sealed class CreatePasskeyRequest
{
    public required string Subject { get; set; }
    public required string DisplayName { get; set; }
    public required PublicKeyCredentialCreationResponse Response { get; set; }
}