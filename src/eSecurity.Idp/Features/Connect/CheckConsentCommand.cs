using System.Text.Json.Serialization;
using eSecurity.Core.Responses;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authorization.Consents;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Connect;

public record CheckConsentCommand : IRequest<Result>
{
    [JsonPropertyName("client_id")]
    public required Guid ClientId { get; set; }
    
    [JsonPropertyName("scopes")]
    public required List<string> Scopes { get; set; }
}

public class CheckConsentCommandHandler(
    ISessionQueryService sessionQueryService,
    IUserQueryService userQueryService,
    IClientQueryService clientQueryService,
    IConsentQueryService consentQueryService,
    ISessionAccessor sessionAccessor) : IRequestHandler<CheckConsentCommand, Result>
{
    private readonly ISessionQueryService _sessionQueryService = sessionQueryService;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly IConsentQueryService _consentQueryService = consentQueryService;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;

    public async Task<Result> Handle(CheckConsentCommand request, CancellationToken cancellationToken)
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
                Description = "Session not found"
            });
        }
        
        var user = await _userQueryService.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

        var client = await _clientQueryService.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Client not found"
            });
        }

        var consent = await _consentQueryService.GetByClientAsync(user.Id, client.Id, cancellationToken);
        if (consent is null)
        {
            return Results.Success(SuccessCodes.Ok, new CheckConsentResponse
            {
                IsGranted = false,
                UserHint = user.Username,
                RemainingScopes = request.Scopes
            });
        }

        if (!consent.HasScopes(request.Scopes, out var remainingScopes))
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