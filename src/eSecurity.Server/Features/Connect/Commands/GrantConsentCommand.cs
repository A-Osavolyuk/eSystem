using System.Text.Json;
using eSecurity.Core.Common.Requests;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.OAuth.Consents;
using eSecurity.Server.Security.Cookies;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Features.Connect.Commands;

public record GrantConsentCommand(GrantConsentRequest Request) : IRequest<Result>;

public class GrantConsentCommandHandler(
    IUserManager userManager,
    IClientManager clientManager,
    IConsentManager consentManager,
    ISessionManager sessionManager,
    IHttpContextAccessor httpContextAccessor,
    IOptions<OpenIdConfiguration> options,
    IDataProtectionProvider protectionProvider) : RequestHandlerBase<GrantConsentCommand, Result>(httpContextAccessor)
{
    private readonly IUserManager _userManager = userManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly OpenIdConfiguration _options = options.Value;

    public override async Task<Result> Handle(GrantConsentCommand request, CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Cookies.TryGetValue(DefaultCookies.Session, out var cookie) ||
            string.IsNullOrEmpty(cookie))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.LoginRequired,
                Description = "Login required"
            });
        }

        SessionCookie? sessionCookie;
        try
        {
            var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Session);
            var unprotectedCookie = protector.Unprotect(cookie);
            
            sessionCookie = JsonSerializer.Deserialize<SessionCookie>(unprotectedCookie);
            if (sessionCookie is null)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.LoginRequired,
                    Description = "Login required"
                });
            }
        }
        catch (Exception)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.LoginRequired,
                Description = "Login required"
            });
        }
        
        var session = await _sessionManager.FindByIdAsync(sessionCookie.SessionId, cancellationToken);
        if (session is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Session was not found"
            });
        }
        
        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User was not found"
            });
        }

        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Client was not found"
            });
        }

        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null)
        {
            consent = new ConsentEntity
            {
                UserId = user.Id,
                ClientId = client.Id,
            };

            var createResult = await _consentManager.CreateAsync(consent, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            foreach (var scope in request.Request.Scopes)
            {
                if (!_options.ScopesSupported.Contains(scope))
                {
                    return Results.ClientError(ClientErrorCode.BadRequest, new Error
                    {
                        Code = ErrorCode.InvalidScope,
                        Description = $"'{scope}' scope is not supported."
                    });
                }

                var clientScope = client.AllowedScopes.FirstOrDefault(x => x.Scope.Value == scope);
                if (clientScope is null)
                {
                    return Results.ClientError(ClientErrorCode.BadRequest, new Error
                    {
                        Code = ErrorCode.InvalidScope,
                        Description = $"'{scope}' scope is not supported by client."
                    });
                }
                
                var grantResult = await _consentManager.GrantAsync(consent, clientScope, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }

            return Results.Success(SuccessCodes.Ok);
        }

        if (!consent.HasScopes(request.Request.Scopes, out var remainingScopes))
        {
            foreach (var scope in remainingScopes)
            {
                if (!_options.ScopesSupported.Contains(scope))
                {
                    return Results.ClientError(ClientErrorCode.BadRequest, new Error
                    {
                        Code = ErrorCode.InvalidScope,
                        Description = $"'{scope}' scope is not supported."
                    });
                }

                var clientScope = client.AllowedScopes.First(x => x.Scope.Value == scope);
                var grantResult = await _consentManager.GrantAsync(consent, clientScope, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}