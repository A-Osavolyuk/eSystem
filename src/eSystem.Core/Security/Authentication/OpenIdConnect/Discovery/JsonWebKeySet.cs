using Microsoft.IdentityModel.Tokens;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;

public class JsonWebKeySet
{
    public List<JsonWebKey> Keys { get; set; } = [];
}