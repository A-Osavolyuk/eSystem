namespace eSecurity.Core.Common.Requests;

public sealed class ConfirmForgotPasswordRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}