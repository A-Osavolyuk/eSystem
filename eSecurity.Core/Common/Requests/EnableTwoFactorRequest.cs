namespace eSecurity.Core.Common.Requests;

public class EnableTwoFactorRequest
{
    public required Guid UserId { get; set; }
}