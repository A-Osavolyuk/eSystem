namespace eSecurity.Core.Common.Responses;

public class CheckConsentResponse
{
    public required bool IsGranted { get; set; }
    public required string UserHint { get; set; }
    public List<string> RemainingScopes { get; set; } = [];
}