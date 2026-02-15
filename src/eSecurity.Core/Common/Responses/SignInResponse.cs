namespace eSecurity.Core.Common.Responses;

public sealed class SignInResponse
{
    public Guid TransactionId { get; set; }
    public Guid? SessionId { get; set; }
}