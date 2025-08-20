using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Types;

public class AuthenticatorSelection
{
    [JsonPropertyName("authenticatorAttachment")]
    public AuthenticatorAttachment AuthenticatorAttachment { get; set; } =  AuthenticatorAttachment.Platform;
    
    [JsonPropertyName("residentKey")]
    public ResidentKey ResidentKey { get; set; } = ResidentKey.Preferred;
    
    [JsonPropertyName("userVerification")]
    public UserVerification UserVerification { get; set; } = UserVerification.Preferred;
}