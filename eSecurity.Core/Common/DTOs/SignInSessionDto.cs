using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.SignIn.Session;
using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.DTOs;

public class SignInSessionDto
{
    [JsonPropertyName("sid")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("user_id")]
    public Guid? UserId { get; set; }
    
    [JsonPropertyName("next_step")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SignInStep NextStep { get; set; }

    [JsonPropertyName("providers")]
    public string? Provider { get; set; }

    [JsonPropertyName("oauth_flow")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OAuthFlow? OAuthFlow { get; set; }
}