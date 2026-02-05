using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;

public sealed class ClientCredentialsResponse : TokenResponse
{
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}