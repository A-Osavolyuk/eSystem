using eSecurity.Core.Security.Authentication.SignIn.Session;
using eSystem.Core.Data.Entities;
using OAuthFlow = eSecurity.Core.Security.Authorization.OAuth.OAuthFlow;

namespace eSecurity.Server.Data.Entities;

public class SignInSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    public IReadOnlyCollection<SignInStep> RequiredSteps { get; set; } = [];
    public HashSet<SignInStep> CompletedSteps { get; set; } = [];
    public SignInStep CurrentStep { get; set; }
    public SignInStatus Status { get; set; }
    
    public string? Provider { get; set; }
    public OAuthFlow? OAuthFlow { get; set; }
    
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset ExpireDate { get; set; }
    public bool IsActive => ExpireDate > DateTimeOffset.UtcNow;

    public UserEntity? User { get; set; }
}