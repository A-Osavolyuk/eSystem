namespace eSecurity.Core.Common.Requests;

public class DisableTwoFactorRequest
{
    public required Guid UserId { get; set; }
}