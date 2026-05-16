using System.Text.Json.Serialization;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Token.DeviceCode;

public sealed class DeviceCodeResponse : TokenResponse
{
    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [JsonPropertyName("login_token_hint")] 
    public string? LoginTokenHint { get; set; }
}