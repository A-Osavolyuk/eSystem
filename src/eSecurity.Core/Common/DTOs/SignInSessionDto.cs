using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.SignIn.Session;
using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Common.DTOs;

public class SignInSessionDto
{
    [JsonPropertyName("sid")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("userId")]
    public Guid? UserId { get; set; }
    
    [JsonPropertyName("nextStep")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SignInStep NextStep { get; set; }

    [JsonPropertyName("providers")]
    public string? Provider { get; set; }

    [JsonPropertyName("oauthFlow")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OAuthFlow? OAuthFlow { get; set; }
}