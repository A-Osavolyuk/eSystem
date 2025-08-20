using System.Text.Json.Serialization;
using eShop.Domain.Constants;

namespace eShop.Domain.Types;

public class AuthenticatorSelection
{
    [JsonPropertyName("authenticatorAttachment")]
    public string AuthenticatorAttachment { get; set; } = AuthenticatorAttachments.Platform;

    [JsonPropertyName("residentKey")] 
    public string ResidentKey { get; set; } = ResidentKeys.Preferred;

    [JsonPropertyName("userVerification")]
    public string UserVerification { get; set; } = UserVerifications.Preferred;
}