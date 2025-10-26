using eSystem.Core.Security.Verification;

namespace eAccount.Blazor.Server.Domain.Common;

public class ConfirmationContext
{
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}