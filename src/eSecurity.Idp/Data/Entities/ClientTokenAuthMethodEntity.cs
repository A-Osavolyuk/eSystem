using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class ClientTokenAuthMethodEntity : Entity
{
    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;

    public Guid MethodId { get; set; }
    public TokenAuthMethodEntity Method { get; set; } = null!;
}