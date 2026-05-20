using System.Security.Claims;
using eCinema.Server.Data.Entities;
using eCinema.Server.Security.Authentication.OpenIdConnect.Session;
using eCinema.Server.Security.Cryptography.Protection.Constants;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

namespace eCinema.Server.Security.Authentication.Ticket;

public class SessionTicketStore(
    IServiceScopeFactory scopeFactory,
    IDataProtectionProvider protectionProvider) : ITicketStore
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector(ProtectionPurposes.Token);

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<ISessionManager>();
        
        var session = new SessionEntity()
        {
            Id = Guid.CreateVersion7(),
            SessionKey = Guid.CreateVersion7().ToString("N"),
            UserId = ticket.Principal.Claims.First(x => x.Type == AppClaimTypes.Sub).Value,
            Sid = ticket.Principal.Claims.First(x => x.Type == AppClaimTypes.Sid).Value,
        };

        session.Properties = new SessionPropertiesEntity()
        {
            Id = Guid.CreateVersion7(),
            SessionId = session.Id,
            AllowRefresh = ticket.Properties.AllowRefresh,
            IsPersistent = ticket.Properties.IsPersistent,
            IssuedUtc = ticket.Properties.IssuedUtc,
            ExpiresUtc = ticket.Properties.ExpiresUtc,
            RedirectUri = ticket.Properties.RedirectUri
        };
        
        session.Claims = ticket.Principal.Claims.Select(claim => new SessionClaimEntity()
        {
            Id = Guid.CreateVersion7(),
            SessionId = session.Id,
            Type = claim.Type,
            Value = claim.Value,
        }).ToList();

        session.Tokens = ticket.Properties.Items.Select(item => new SessionTokenEntity()
        {
            Id = Guid.CreateVersion7(),
            SessionId = session.Id,
            TokenType = item.Key,
            EncryptedValue = _protector.Protect(item.Value!)
        }).ToList();
        
        await manager.CreateAsync(session);
        return session.SessionKey;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<ISessionManager>();
        
        var session = await manager.FindByKeyAsync(key);
        if (session is not null)
        {
            session.Properties.ExpiresUtc = ticket.Properties.ExpiresUtc;
            await manager.UpdateAsync(session);
        }
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<ISessionManager>();
        
        var session = await manager.FindByKeyAsync(key);
        if (session is null) 
            return null;

        if (session.Properties.ExpiresUtc.HasValue && session.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow)
        {
            await manager.DeleteAsync(session);
            return null;
        }

        var claims = session.Claims.Select(x => new Claim(x.Type, x.Value));
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authenticationProperties = new AuthenticationProperties()
        {
            AllowRefresh = session.Properties.AllowRefresh,
            ExpiresUtc = session.Properties.ExpiresUtc,
            IsPersistent = session.Properties.IsPersistent,
            IssuedUtc = session.Properties.IssuedUtc,
            RedirectUri = session.Properties.RedirectUri
        };

        authenticationProperties.Items.Clear();
        foreach (var token in session.Tokens)
        {
            authenticationProperties.Items.Add(token.TokenType, _protector.Unprotect(token.EncryptedValue));
        }

        return new AuthenticationTicket(
            claimsPrincipal,
            authenticationProperties,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }

    public async Task RemoveAsync(string key)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<ISessionManager>();
        var session = await manager.FindByKeyAsync(key);
        if (session is not null)
        {
            await manager.DeleteAsync(session);
        }
    }
}