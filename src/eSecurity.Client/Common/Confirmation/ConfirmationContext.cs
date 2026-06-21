using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Client.Common.Confirmation;

public class ConfirmationContext
{
    public required OperationType Operation  { get; set; }
}