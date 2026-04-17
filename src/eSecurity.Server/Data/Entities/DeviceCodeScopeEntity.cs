using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class DeviceCodeScopeEntity : Entity
{
    public Guid Id { get; set; }

    public required string Scope { get; set; }

    public Guid DeviceCodeId { get; set; }
    public DeviceCodeEntity DeviceCode { get; set; } = null!;
}