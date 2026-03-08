namespace eSecurity.Core.Common.Requests;

public sealed class SetUsernameRequest
{
    [JsonPropertyName("session_id")]
    public required Guid SessionId { get; set; }
    
    [JsonPropertyName("username")]
    public required string Username { get; set; }
}