using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Server.Data.Entities;

public sealed class CibaRequestEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string AuthReqId { get; set; }
    public required string Scope { get; set; }
    public required int Interval { get; set; }
    public required CibaRequestState State { get; set; }
    
    public required DateTimeOffset ExpiresAt { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ConsumedAt { get; set; }
    
    public string? UserCode { get; set; }
    public string? AcrValues { get; set; }
    public string? BindingMessage { get; set; }
    public string? DeniedReason { get; set; }

    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    
    public Guid? SessionId { get; set; }
    public SessionEntity? Session { get; set; }
}