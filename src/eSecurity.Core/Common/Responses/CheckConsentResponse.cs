namespace eSecurity.Core.Common.Responses;

public class CheckConsentResponse
{
    public required bool Granted { get; set; }
    public List<string> RemainingScopes { get; set; } = [];
}