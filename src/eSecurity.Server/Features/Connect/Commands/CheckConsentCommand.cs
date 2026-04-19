using System.Text.Json;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.OAuth.Consents;
using eSecurity.Server.Security.Cookies;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Features.Connect.Commands;

public record CheckConsentCommand(CheckConsentRequest Request) : IRequest<Result>;

public class CheckConsentCommandHandler(
    IConsentManager consentManager,
    ISessionManager sessionManager,
    IClientManager clientManager,
    IUserManager userManager,
    IDataProtectionProvider protectionProvider,
    IHttpContextAccessor httpContextAccessor) : RequestHandlerBase<CheckConsentCommand, Result>(httpContextAccessor)
{
    private readonly IConsentManager _consentManager = consentManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;

    public override async Task<Result> Handle(CheckConsentCommand request, CancellationToken cancellationToken)
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
                Description = "Session not found"
            });
        }
        
        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Client not found"
            });
        }

        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null)
        {
            return Results.Success(SuccessCodes.Ok, new CheckConsentResponse
            {
                IsGranted = false,
                UserHint = user.Username,
                RemainingScopes = request.Request.Scopes
            });
        }

        if (!consent.HasScopes(request.Request.Scopes, out var remainingScopes))
        {
            return Results.Success(SuccessCodes.Ok, new CheckConsentResponse
            {
                IsGranted = false,
                UserHint = user.Username,
                RemainingScopes = remainingScopes.ToList()
            });
        }
        
        return Results.Success(SuccessCodes.Ok, new CheckConsentResponse
        {
            IsGranted = true, 
            UserHint = user.Username,
        });
    }
}