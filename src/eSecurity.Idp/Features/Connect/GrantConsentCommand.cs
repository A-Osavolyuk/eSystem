using System.Text.Json.Serialization;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authorization.Consents;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Idp.Features.Connect;

public record GrantConsentCommand : IRequest<Result>
{
    [JsonPropertyName("client_id")]
    public required Guid ClientId { get; set; }
    
    [JsonPropertyName("scopes")]
    public required List<string> Scopes { get; set; }
}

public class GrantConsentCommandHandler(
    IUserQueryService userQueryService,
    IConsentManager consentManager,
    IClientQueryService clientQueryService,
    IOptions<OpenIdConfiguration> options,
    ISessionQueryService sessionQueryService,
    ISessionAccessor sessionAccessor) : IRequestHandler<GrantConsentCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly ISessionQueryService _sessionQueryService = sessionQueryService;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly OpenIdConfiguration _options = options.Value;

    public async Task<Result> Handle(GrantConsentCommand request, CancellationToken cancellationToken)
    {
        var sessionCookie = _sessionAccessor.GetCookie();
        if (sessionCookie is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.LoginRequired,
                Description = "Login required"
            });
        }
        
        var session = await _sessionQueryService.GetByIdAsync(sessionCookie.SessionId, cancellationToken);
        if (session is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Session was not found"
            });
        }
        
        var user = await _userQueryService.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User was not found"
            });
        }

        var client = await _clientQueryService.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Client was not found"
            });
        }

        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(
            client, cancellationToken);
        
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
            
            foreach (var scope in request.Scopes)
            {
                if (!_options.ScopesSupported.Contains(scope))
                {
                    return Results.ClientError(ClientErrorCode.BadRequest, new Error
                    {
                        Code = ErrorCode.InvalidScope,
                        Description = $"'{scope}' scope is not supported."
                    });
                }
                
                var clientScope = clientScopes.FirstOrDefault(x => x.Scope.Value == scope);
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

        if (!consent.HasScopes(request.Scopes, out var remainingScopes))
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

                var clientScope = clientScopes.First(x => x.Scope.Value == scope);
                var grantResult = await _consentManager.GrantAsync(consent, clientScope, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}