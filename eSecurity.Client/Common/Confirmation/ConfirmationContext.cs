using eSecurity.Core.Security.Authorization.Access;

namespace eSecurity.Client.Common.Confirmation;

public class ConfirmationContext
{
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}