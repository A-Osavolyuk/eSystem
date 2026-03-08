namespace eSecurity.Core.Common.Responses;

public class AuthorizeResponse
{
    [JsonPropertyName("state")]
    public required string State { get; set; }
    
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}