using System.Text.Json.Serialization;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authorization.Consents;
using eSecurity.Idp.Security.Authorization.Token.DeviceCode;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.DeviceCode;

public sealed record DeviceCodeDecisionCommand : IRequest<Result>
{
    [JsonPropertyName("user_code")]
    public string? UserCode { get; set; }
    
    [JsonPropertyName("decision")]
    public DeviceCodeDecision Decision { get; set; }

    [JsonPropertyName("deny_reason")]
    public string? DenyReason { get; set; }
}

public sealed class DeviceCodeDecisionCommandHandler(
    IDeviceCodeManager deviceCodeManager,
    ISessionManager sessionManager,
    ICurrentUserAccessor currentUserAccessor,
    IConsentManager consentManager,
    IClientQueryService clientQueryService,
    ISessionAccessor sessionAccessor) 
    : IRequestHandler<DeviceCodeDecisionCommand, Result>
{
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;

    public async Task<Result> Handle(DeviceCodeDecisionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserCode))
            throw new ValidationException("UserCode is required");
        
        var deviceCode = await _deviceCodeManager.FindByCodeAsync(request.UserCode, cancellationToken);
        if (deviceCode is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Device code was not found"
            });
        }
        
        if (deviceCode.ExpiresAt < DateTimeOffset.UtcNow)
        {
            deviceCode.State = DeviceCodeState.Expired;
            var result = await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
            if (!result.Succeeded) return result;
            
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.ExpiredToken,
                Description = "Device code is already expired"
            });
        }
        
        if (deviceCode.State != DeviceCodeState.Pending)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidToken,
                Description = "Device code is not valid"
            });
        }

        var sessionCookie = _sessionAccessor.GetCookie();
        if (sessionCookie is not null)
        {
            var session = await _sessionManager.FindByIdAsync(sessionCookie.SessionId, cancellationToken);
            if (session is null)
            {
                return Results.ClientError(ClientErrorCode.NotFound, new Error
                {
                    Code = ErrorCode.NotFound,
                    Description = "Session not found"
                });
            }

            deviceCode.SessionId = session.Id;
        }

        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        deviceCode.UserId = user.Id;

        if (request.Decision == DeviceCodeDecision.Denied)
        {
            deviceCode.State = DeviceCodeState.Denied;
            deviceCode.DeniedAt = DateTimeOffset.UtcNow;
            deviceCode.DenyReason = request.DenyReason;

            var result = await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
            return result.Succeeded ? result : Results.Success(SuccessCodes.Ok);
        }
        
        var client = await _clientQueryService.GetByIdAsync(deviceCode.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Client not found"
            });
        }

        var allowedScopes = await _clientQueryService.GetAllowedScopesAsync(
            client, cancellationToken);
        
        var scopes = deviceCode.Scopes.Select(x => x.Scope).ToList();
        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null)
        {
            var clientScopes = allowedScopes
                .Where(x => scopes.Contains(x.Scope.Value))
                .ToList();

            consent = new ConsentEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ClientId = client.Id,
            };
            
            consent.GrantedScopes = clientScopes.Select(x => new GrantedScopeEntity
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
                var clientScopes = allowedScopes
                    .Where(x => remainingScopes.Contains(x.Scope.Value))
                    .ToList();
                
                consent.GrantedScopes = clientScopes.Select(x => new GrantedScopeEntity
                {
                    Id = Guid.CreateVersion7(),
                    ConsentId = consent.Id,
                    ClientScopeId = x.Id,
                }).ToList();
                
                var consentResult = await _consentManager.UpdateAsync(consent, cancellationToken);
                if (!consentResult.Succeeded) return consentResult;
            }
        }
        
        deviceCode.State = DeviceCodeState.Approved;
        deviceCode.ApprovedAt = DateTimeOffset.UtcNow;
        
        return await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
    }
}

public sealed class DeviceCodeDecisionCommandValidator : IRequestValidator<DeviceCodeDecisionCommand>
{
    public async ValueTask<Result> Validate(DeviceCodeDecisionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.UserCode))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'user_code' is required"
            });
        }

        if (request.Decision == DeviceCodeDecision.None)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'decision' is invalid"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}