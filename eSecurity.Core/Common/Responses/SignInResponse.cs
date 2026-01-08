using System.Text.Json.Serialization;

namespace eSecurity.Core.Common.Responses;

public sealed class SignInResponse
{
    [JsonPropertyName("sid")]
    public Guid SessionId { get; set; }
}