using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Data.Entities;

public sealed class DeviceCodeAcrValueEntity : Entity
{
    public Guid Id { get; set; }
    
    public AuthenticationContextClassReference Value { get; set; }

    public Guid DeviceCodeId { get; set; }
    public DeviceCodeEntity DeviceCode { get; set; } = null!;
}