namespace eSecurity.Core.Common.Responses;

public sealed class SignInResponse
{
    public Guid UserId { get; set; }
    public bool RequireTwoFactor { get; set; }
}