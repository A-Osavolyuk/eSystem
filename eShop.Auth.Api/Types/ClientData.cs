using System.Text.Json;
using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Types;

public class ClientData
{
    [JsonPropertyName("type")] 
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = string.Empty;
    
    [JsonPropertyName("origin")]
    public string Origin { get; set; } = string.Empty;
    
    [JsonPropertyName("crossOrigin")]
    public bool CrossOrigin { get; set; }

    public static ClientData? Parse(string clientDataJson)
    {
        var bytes = CredentialUtils.Base64UrlDecode(clientDataJson);
        var json = Encoding.UTF8.GetString(bytes);
        var clientData = JsonSerializer.Deserialize<ClientData>(json);
        
        return clientData;
    }
}