namespace eSecurity.Core.Common.Requests;

public class RevokeRecoveryCodesRequest
{
    public required Guid UserId { get; set; }
}