using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.BackchannelAuthentication;
using eSecurity.Idp.Security.Authentication.Ciba;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Cryptography;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Idp.Features.Connect;

public sealed record BackchannelAuthenticationCommand : IRequest<Result>
{
    [FromForm(Name = "scope")]
    public string? Scope { get; set; }

    [FromForm(Name = "client_id")]
    public string? ClientId { get; set; }
    
    [FromForm(Name = "client_secret")]
    public string? ClientSecret { get; set; }
    
    [FromForm(Name = "client_assertion")]
    public string? ClientAssertion { get; set; }
    
    [FromForm(Name = "client_assertion_type")]
    public AssertionType? ClientAssertionType { get; set; }
    
    [FromForm(Name = "login_hint")]
    public string? LoginHint { get; set; }
    
    [FromForm(Name = "login_token_hint")]
    public string? LoginTokenHint { get; set; }
    
    [FromForm(Name = "id_token_hint")]
    public string? IdTokenHint { get; set; }
    
    [FromForm(Name = "binding_message")]
    public string? BindingMessage { get; set; }
    
    [FromForm(Name = "requested_expiry")]
    public int? RequestedExpiry { get; set; }
    
    [FromForm(Name = "acr_values")]
    public string? AcrValues { get; set; }

    [FromForm(Name = "client_notification_token")]
    public string? ClientNotificationToken { get; set; }
}

public sealed class BackchannelAuthenticationCommandHandler(
    IClientQueryService clientQueryService,
    IUserResolverProvider userResolverProvider,
    ICibaRequestManager cibaRequestManager,
    IOptions<BackchannelAuthenticationOptions> options) : IRequestHandler<BackchannelAuthenticationCommand, Result>
{
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly IUserResolverProvider _userResolverProvider = userResolverProvider;
    private readonly ICibaRequestManager _cibaRequestManager = cibaRequestManager;
    private readonly BackchannelAuthenticationOptions _options = options.Value;

    public async Task<Result> Handle(BackchannelAuthenticationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ClientId))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "client_id is required"
            });
        }

        var client = await _clientQueryService.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.InvalidClient,
                Description = "Invalid client"
            });
        }

        var clientGrantTypes = await _clientQueryService.GetSupportedGrantTypesAsync(
            client, cancellationToken);
        
        if (clientGrantTypes.All(x => x.Grant.Grant != GrantType.Ciba))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.UnauthorizedClient,
                Description = "CIBA grant type is not allowed"
            });
        }

        if (client.NotificationDeliveryMode == NotificationDeliveryMode.None)
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.UnauthorizedClient,
                Description = "Notification delivery mode is not configured"
            });
        }

        var clientUris = await _clientQueryService.GetUrisAsync(client, cancellationToken);
        if (client.NotificationDeliveryMode is NotificationDeliveryMode.Ping or NotificationDeliveryMode.Push &&
            clientUris.All(x => x.Type != UriType.NotificationEndpoint))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error
            {
                Code = ErrorCode.UnauthorizedClient,
                Description = "Client notification endpoint is not configured"
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.Scope))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "scope is required"
            });
        }

        var scopes = request.Scope.Split(' ').ToList();
        if (!scopes.Contains(ScopeTypes.OpenId))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidScope,
                Description = "openid scope is mandatory for backchannel authentication"
            });
        }

        var allowedScopes = await _clientQueryService.GetAllowedScopesAsync(
            client, cancellationToken);

        var unsupportedScopes = scopes
            .Except(allowedScopes.Select(x => x.Scope.Value))
            .ToList();
        
        if (unsupportedScopes.Count > 0)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidScope,
                Description = $"'{string.Join(", ", unsupportedScopes)}' scopes are not allowed for this client"
            });
        }

        var hintsCount =
            (string.IsNullOrWhiteSpace(request.LoginHint) ? 0 : 1) +
            (string.IsNullOrWhiteSpace(request.LoginTokenHint) ? 0 : 1) +
            (string.IsNullOrWhiteSpace(request.IdTokenHint) ? 0 : 1);

        if (hintsCount == 0)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "One of login_hint, login_token_hint or id_token_hint must be provided"
            });
        }

        if (hintsCount > 1)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Multiple hints are not allowed"
            });
        }

        UserHint? hint = request switch
        {
            { IdTokenHint: not null and not "" } => UserHint.IdTokenHint,
            { LoginTokenHint: not null and not "" } => UserHint.LoginTokenHint,
            { LoginHint: not null and not "" } => UserHint.LoginHint,
            _ => null
        };

        if (hint is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Invalid hint"
            });
        }

        var userResolver = _userResolverProvider.GetResolver(hint.Value);
        var resolveResult = await userResolver.ResolveAsync(request, cancellationToken);
        if (!resolveResult.Succeeded)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, resolveResult.GetError());
        }

        if (!resolveResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnknownUserId,
                Description = "Unknown user"
            });
        }
        
        if (request.RequestedExpiry is not null &&
            (request.RequestedExpiry > _options.MaxRequestLifetime.TotalSeconds ||
             request.RequestedExpiry < _options.MinRequestLifetime.TotalSeconds))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = $"requested_expiry must be between {_options.MinRequestLifetime} " +
                              $"and {_options.MaxRequestLifetime} seconds"
            });
        }
        
        var requestedExpiry = request.RequestedExpiry.HasValue 
            ? TimeSpan.FromSeconds(request.RequestedExpiry.Value) 
            : _options.DefaultRequestLifetime;

        if (!string.IsNullOrWhiteSpace(request.BindingMessage) && request.BindingMessage.Length > 255)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidBindingMessage,
                Description = "binding_message must not be longer then 255 characters"
            });
        }
        
        var cibaRequest = new CibaRequestEntity
        {
            Id = Guid.CreateVersion7(),
            AuthReqId = RandomKeyFactory.Create(_options.AuthReqIdLength),
            ClientId = client.Id,
            UserId = user.Id,
            State = CibaRequestState.Pending,
            Interval = _options.Interval,
            Scope = request.Scope,
            AcrValues = request.AcrValues,
            BindingMessage = request.BindingMessage,
            ClientNotificationToken = request.ClientNotificationToken,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.Add(requestedExpiry),
        };

        var result = await _cibaRequestManager.CreateAsync(cibaRequest, cancellationToken);
        if (!result.Succeeded) return result;
        
        return Results.Success(SuccessCodes.Ok, new BackchannelAuthenticationResponse
        {
            AuthReqId = cibaRequest.AuthReqId,
            Interval = cibaRequest.Interval,
            ExpiresIn = (int)requestedExpiry.TotalSeconds,
        });
    }
}