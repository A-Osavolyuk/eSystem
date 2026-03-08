namespace eSecurity.Core.Common.Requests;

public sealed class ResetPasswordRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}