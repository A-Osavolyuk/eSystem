namespace eSecurity.Core.Common.Responses;

public sealed class CompleteSignUpResponse
{
    [JsonPropertyName("session_id")]
    public Guid SessionId { get; set; }
}