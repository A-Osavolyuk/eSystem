using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class DeviceCodeScopeEntity : Entity
{
    public Guid Id { get; set; }

    public required string Scope { get; set; }

    public Guid DeviceCodeId { get; set; }
    public DeviceCodeEntity DeviceCode { get; set; } = null!;
}