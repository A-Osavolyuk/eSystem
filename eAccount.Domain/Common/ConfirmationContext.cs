using eSystem.Core.Security.Authorization.Access;

namespace eAccount.Domain.Common;

public class ConfirmationContext
{
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}