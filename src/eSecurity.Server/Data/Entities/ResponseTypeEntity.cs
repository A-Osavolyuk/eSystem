using eSystem.Core.Server.Data.Entities;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Data.Entities;

public sealed class ResponseTypeEntity : Entity
{
    public Guid Id { get; set; }
    public required ResponseType Type { get; set; }
}