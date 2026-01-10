using Microsoft.IdentityModel.Tokens;

namespace eSecurity.Core.Security.Authentication.Oidc;

public class JsonWebKeySet
{
    public List<JsonWebKey> Keys { get; set; } = [];
}