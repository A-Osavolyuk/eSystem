using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.SignIn.Session;

namespace eSecurity.Core.Common.DTOs;

public class SignInSessionDto
{
    [JsonPropertyName("sid")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }
    
    [JsonPropertyName("next_step")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SignInStep NextStep { get; set; }
}