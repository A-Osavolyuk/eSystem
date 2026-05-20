using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class DeviceCodeAcrValueEntity : Entity
{
    public Guid Id { get; set; }
    
    public AuthenticationContextClassReference Value { get; set; }

    public Guid DeviceCodeId { get; set; }
    public DeviceCodeEntity DeviceCode { get; set; } = null!;
}