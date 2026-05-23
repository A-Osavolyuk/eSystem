using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class ConfirmEndSessionRequest
{
    [JsonPropertyName("end_session_request_id")]
    public Guid RequestId { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }
}