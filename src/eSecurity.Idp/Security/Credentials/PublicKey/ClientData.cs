using System.Text.Json;
using System.Text.Json.Serialization;
using eSecurity.WebAuthN.Constants;
using Microsoft.AspNetCore.WebUtilities;

namespace eSecurity.Idp.Security.Credentials.PublicKey;

public sealed class ClientData
{
    [JsonPropertyName("type")] 
    public ClientDataType Type { get; set; }
    
    [JsonPropertyName("challenge")]
    public byte[] Challenge { get; set; } = null!;
    
    [JsonPropertyName("origin")]
    public string Origin { get; set; } = string.Empty;
    
    [JsonPropertyName("crossOrigin")]
    public bool CrossOrigin { get; set; }

    public static ClientData? Parse(byte[] clientDataBytes)
    {
        var clientDataBytesJson = Encoding.UTF8.GetString(clientDataBytes);
        var clientData = JsonSerializer.Deserialize<ClientData>(clientDataBytesJson);
        
        return clientData;
    }
}