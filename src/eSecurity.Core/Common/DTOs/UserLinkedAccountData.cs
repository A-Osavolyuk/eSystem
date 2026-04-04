namespace eSecurity.Core.Common.DTOs;

public class UserLinkedAccountData
{
    [JsonPropertyName("google_connected")]
    public bool GoogleConnected { get; set; }
    
    [JsonPropertyName("facebook_connected")]
    public bool FacebookConnected { get; set; }
    
    [JsonPropertyName("microsoft_connected")]
    public bool MicrosoftConnected { get; set; }
    
    [JsonPropertyName("x_connected")]
    public bool XConnected { get; set; }
    
    [JsonPropertyName("linked_accounts")]
    public List<UserLinkedAccountDto> LinkedAccounts { get; set; } = [];
}