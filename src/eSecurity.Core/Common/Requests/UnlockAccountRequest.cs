namespace eSecurity.Core.Common.Requests;

public class UnlockAccountRequest
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
}