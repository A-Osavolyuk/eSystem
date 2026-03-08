namespace eSecurity.Core.Common.Requests;

public sealed class SignUpRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("username")]
    public required string Username { get; set; }
    
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}