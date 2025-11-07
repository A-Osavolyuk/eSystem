namespace eSecurity.Security.Authorization.Access;

public class ConfirmationContext
{
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}