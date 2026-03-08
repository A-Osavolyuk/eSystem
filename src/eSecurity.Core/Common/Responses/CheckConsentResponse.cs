namespace eSecurity.Core.Common.Responses;

public class CheckConsentResponse
{
    [JsonPropertyName("is_granted")]
    public required bool IsGranted { get; set; }
    
    [JsonPropertyName("user_hint")]
    public required string UserHint { get; set; }
    
    [JsonPropertyName("remaining_scopes")]
    public List<string> RemainingScopes { get; set; } = [];
}