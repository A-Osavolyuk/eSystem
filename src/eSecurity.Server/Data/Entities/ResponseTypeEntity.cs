using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Data.Entities;

public sealed class ResponseTypeEntity : Entity
{
    public Guid Id { get; set; }
    public required ResponseType Type { get; set; }
}