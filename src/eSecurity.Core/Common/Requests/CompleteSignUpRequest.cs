namespace eSecurity.Core.Common.Requests;

public sealed class CompleteSignUpRequest
{
    public required Guid TransactionId { get; set; }
    public required string Code { get; set; }
}