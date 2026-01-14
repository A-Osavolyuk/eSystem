using System.Text.Json.Serialization;

namespace eCinema.Server.Security.Authentication.Oidc;

public class AuthorizationState
{
    [JsonPropertyName("redirect_uri")]
    public required string RedirectUri { get; set; }
}