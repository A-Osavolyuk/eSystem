namespace eSecurity.Core.Common.Responses;

public class LogoutResponse
{
    public required string? State { get; set; }
    public required string PostLogoutRedirectUri  { get; set; }
    public List<string> FrontChannelLogoutUris { get; set; } = [];
}