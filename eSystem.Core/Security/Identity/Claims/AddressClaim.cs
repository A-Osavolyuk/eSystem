using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Identity.Claims;

public class AddressClaim
{
    [JsonPropertyName("street_address")]
    public required string StreetAddress { get; set; }
    
    [JsonPropertyName("locality")]
    public required string Locality { get; set; }
    
    [JsonPropertyName("region")]
    public required string Region { get; set; }
    
    [JsonPropertyName("postal_code")]
    public required string PostalCode { get; set; }
    
    [JsonPropertyName("country")]
    public required string Country { get; set; }
}