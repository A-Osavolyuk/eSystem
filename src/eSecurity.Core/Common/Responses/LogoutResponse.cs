namespace eSecurity.Core.Common.Responses;

public class LogoutResponse
{
    public required string State { get; set; }
    public required List<string> Uris { get; set; }
}