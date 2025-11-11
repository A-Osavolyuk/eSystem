namespace eSecurity.Core.Common.Requests;

public class GenerateRecoveryCodesRequest
{
    public required Guid UserId { get; set; }
}