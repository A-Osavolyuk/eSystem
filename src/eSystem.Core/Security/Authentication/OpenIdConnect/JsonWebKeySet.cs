using Microsoft.IdentityModel.Tokens;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

public class JsonWebKeySet
{
    public List<JsonWebKey> Keys { get; set; } = [];
}