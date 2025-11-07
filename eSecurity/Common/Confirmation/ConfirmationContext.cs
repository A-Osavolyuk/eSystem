using eSecurity.Security.Authorization.Access;

namespace eSecurity.Common.Confirmation;

public class ConfirmationContext
{
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}