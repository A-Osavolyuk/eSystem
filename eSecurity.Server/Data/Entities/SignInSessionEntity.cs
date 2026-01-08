using eSecurity.Server.Security.Authentication.SignIn.Session;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class SignInSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public required IReadOnlyCollection<SignInStep> RequiredSteps { get; init; }
    public required HashSet<SignInStep> CompletedSteps { get; set; }
    public required SignInStep CurrentStep { get; set; }
    public required SignInStatus Status { get; set; }
    
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset ExpireDate { get; set; }
    public bool IsActive => ExpireDate > DateTimeOffset.UtcNow;

    public UserEntity User { get; set; } = null!;
}