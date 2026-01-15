using Microsoft.IdentityModel.Tokens;

namespace eSystem.Core.Security.Authentication.Oidc;

public class JsonWebKeySet
{
    public List<JsonWebKey> Keys { get; set; } = [];
}