using eSecurity.Server.Security.Authentication.Session;
using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Data.Entities;

public sealed class AuthenticationSessionEntity : Entity
{
    public Guid Id { get; set; }

    public string? IdentityProvider { get; set; }
    public OAuthFlow? OAuthFlow { get; set; }

    public bool IsRevoked { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }

    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public bool IsActive => !IsRevoked && ExpiredAt > DateTimeOffset.UtcNow;

    public Guid? UserId { get; set; }
    public UserEntity? User { get; set; }

    public Guid? SessionId { get; set; }
    public SessionEntity? Session { get; set; }

    public ICollection<PassedAuthenticationMethodEntity> PassedMethods { get; set; } = [];
    public ICollection<RequiredAuthenticationMethodEntity> RequiredMethods { get; set; } = [];
    public ICollection<AllowedMfaMethodEntity> AllowedMfaMethods { get; set; } = [];

    public void Pass(params AuthenticationMethod[] methods)
    {
        foreach (var method in methods)
        {
            var passedMethod = RequiredMethods.FirstOrDefault(x => x.Method == method);
            if (passedMethod is not null)
                RequiredMethods.Remove(passedMethod);

            PassedMethods.Add(new PassedAuthenticationMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                SessionId = Id,
                Type = AuthenticationMethodType.Passed,
                Method = method
            });
        }
    }

    public void Require(params AuthenticationMethod[] methods)
    {
        foreach (var method in methods)
        {
            RequiredMethods.Add(new RequiredAuthenticationMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                SessionId = Id,
                Type = AuthenticationMethodType.Required,
                Method = method
            });
        }
    }

    public void AllowMfa(params AuthenticationMethod[] methods)
    {
        foreach (var method in methods)
        {
            AllowedMfaMethods.Add(new AllowedMfaMethodEntity()
            {
                Id = Guid.CreateVersion7(),
                SessionId = Id,
                Type = AuthenticationMethodType.AllowedMfa,
                Method = method
            });
        }
    }
}