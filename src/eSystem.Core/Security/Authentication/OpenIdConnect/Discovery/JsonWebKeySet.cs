using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;

public class JsonWebKeySet
{
    [JsonPropertyName("keys")]
    public List<JsonWebKey> Keys { get; set; } = [];
}