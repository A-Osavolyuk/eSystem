namespace eSecurity.Common.Responses;

public class ForgotPasswordResponse
{
    public Guid UserId { get; set; }
    public bool HasPassword { get; set; }
}