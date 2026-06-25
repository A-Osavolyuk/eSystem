using eSecurity.Core.Security.Authentication.TwoFactor;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class TwoFactorMethodEntity : Entity
{
    public Guid Id { get; set; }
    
    public TwoFactorMethod Type { get; set; }
    
    public int Priority { get; set; }
    
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    
    public bool IsEnabled { get; set; } = true;
}