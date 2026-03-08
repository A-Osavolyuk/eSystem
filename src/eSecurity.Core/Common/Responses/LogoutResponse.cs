namespace eSecurity.Core.Common.Responses;

public class LogoutResponse
{
    [JsonPropertyName("state")]
    public required string? State { get; set; }
    
    [JsonPropertyName("post_logout_redirect_uri")]
    public required string PostLogoutRedirectUri  { get; set; }
    
    [JsonPropertyName("front_channel_logout_uris")]
    public List<string> FrontChannelLogoutUris { get; set; } = [];
}