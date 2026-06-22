using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authentication.SignIn;

public sealed class PasswordSignInPayload : SignInPayload
{
    [JsonPropertyName("login")]
    public required string Login { get; set; }
    
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}