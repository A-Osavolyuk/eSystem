namespace eSecurity.Core.Common.Requests;

public sealed class ChangePasswordRequest
{
    [JsonPropertyName("current_password")]
    public required string CurrentPassword { get; set; }
    
    [JsonPropertyName("new_password")]
    public required string NewPassword { get; set; }
}