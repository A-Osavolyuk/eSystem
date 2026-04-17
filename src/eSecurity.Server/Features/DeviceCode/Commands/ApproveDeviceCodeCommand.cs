using eSecurity.Core.Common.Requests;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.OAuth.Consents;
using eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.DeviceCode.Commands;

public sealed record ApproveDeviceCodeCommand(ApproveDeviceCodeRequest Request) : IRequest<Result>;

public sealed class ApproveDeviceCodeCommandHandler(
    IDeviceCodeManager deviceCodeManager,
    IUserManager userManager,
    ISessionManager sessionManager,
    IConsentManager consentManager,
    IHttpContextAccessor httpContextAccessor,
    IClientManager clientManager) : IRequestHandler<ApproveDeviceCodeCommand, Result>
{
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly IClientManager _clientManager = clientManager;

    public async Task<Result> Handle(ApproveDeviceCodeCommand request, CancellationToken cancellationToken)
    {
        var deviceCode = await _deviceCodeManager.FindByCodeAsync(request.Request.UserCode, cancellationToken);
        if (deviceCode is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Device code not found"
            });
        }

        if (deviceCode.ExpiresAt < DateTimeOffset.UtcNow)
        {
            deviceCode.State = DeviceCodeState.Expired;
            var result = await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
            if (!result.Succeeded) return result;
            
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.ExpiredToken,
                Description = "Device code is already expired"
            });
        }

        if (deviceCode.State != DeviceCodeState.Pending)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidToken,
                Description = "Device code is not valid"
            });
        }

        if (request.Request.SessionId.HasValue)
        {
            var session = await _sessionManager.FindByIdAsync(request.Request.SessionId.Value, cancellationToken);
            if (session is null)
            {
                return Results.ClientError(ClientErrorCode.NotFound, new Error()
                {
                    Code = ErrorCode.NotFound,
                    Description = "Session not found"
                });
            }

            deviceCode.SessionId = session.Id;
        }

        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid request"
            });
        }
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }
        
        var client = await _clientManager.FindByIdAsync(deviceCode.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Client not found"
            });
        }

        var scopes = deviceCode.Scope.Split(' ').ToList();
        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null)
        {
            var clientScopes = client.AllowedScopes
                .Where(x => scopes.Contains(x.Scope.Value))
                .ToList();

            consent = new ConsentEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ClientId = client.Id,
            };
            
            consent.GrantedScopes = clientScopes.Select(x => new GrantedScopeEntity()
            {
                Id = Guid.CreateVersion7(),
                ConsentId = consent.Id,
                ClientScopeId = x.Id,
            }).ToList();
            
            var consentResult = await _consentManager.CreateAsync(consent, cancellationToken);
            if (!consentResult.Succeeded) return consentResult;
        }
        else
        {
            if (!consent.HasScopes(scopes, out var remainingScopes))
            {
                var clientScopes = client.AllowedScopes
                    .Where(x => remainingScopes.Contains(x.Scope.Value))
                    .ToList();
                
                consent.GrantedScopes = clientScopes.Select(x => new GrantedScopeEntity()
                {
                    Id = Guid.CreateVersion7(),
                    ConsentId = consent.Id,
                    ClientScopeId = x.Id,
                }).ToList();
                
                var consentResult = await _consentManager.UpdateAsync(consent, cancellationToken);
                if (!consentResult.Succeeded) return consentResult;
            }
        }

        deviceCode.UserId = user.Id;
        deviceCode.State = DeviceCodeState.Approved;
        
        return await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
    }
}