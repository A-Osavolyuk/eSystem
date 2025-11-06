namespace eSecurity.Common.Responses;

public class AuthorizeResponse
{
    public required Guid UserId { get; set; }
    public required Guid DeviceId { get; set; }
    public required Guid SessionId { get; set; }
    public required string State { get; set; }
    public required string Code { get; set; }
}