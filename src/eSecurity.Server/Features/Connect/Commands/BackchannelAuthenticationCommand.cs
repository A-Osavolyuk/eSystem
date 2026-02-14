using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Ciba;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Cryptography.Keys;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Features.Connect.Commands;

public sealed record BackchannelAuthenticationCommand(BackchannelAuthenticationRequest Request) : IRequest<Result>;

public sealed class BackchannelAuthenticationCommandHandler(
    IClientManager clientManager,
    IUserResolverProvider userResolverProvider,
    ICibaRequestManager cibaRequestManager,
    IKeyFactory keyFactory,
    IOptions<BackchannelAuthenticationOptions> options) : IRequestHandler<BackchannelAuthenticationCommand, Result>
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IUserResolverProvider _userResolverProvider = userResolverProvider;
    private readonly ICibaRequestManager _cibaRequestManager = cibaRequestManager;
    private readonly IKeyFactory _keyFactory = keyFactory;
    private readonly BackchannelAuthenticationOptions _options = options.Value;

    public async Task<Result> Handle(BackchannelAuthenticationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Request.ClientId))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "client_id is required"
            });
        }

        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.Unauthorized(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Invalid client"
            });
        }

        if (!client.HasGrantType(GrantTypes.Ciba))
        {
            return Results.Unauthorized(new Error()
            {
                Code = ErrorTypes.OAuth.UnauthorizedClient,
                Description = "CIBA grant type is not allowed"
            });
        }

        if (client.NotificationDeliveryMode == NotificationDeliveryMode.None)
        {
            return Results.Unauthorized(new Error()
            {
                Code = ErrorTypes.OAuth.UnauthorizedClient,
                Description = "Notification delivery mode is not configured"
            });
        }

        if (client.NotificationDeliveryMode is NotificationDeliveryMode.Ping or NotificationDeliveryMode.Push &&
            !client.HasUri(UriType.NotificationEndpoint))
        {
            return Results.Unauthorized(new Error()
            {
                Code = ErrorTypes.OAuth.UnauthorizedClient,
                Description = "Client notification endpoint is not configured"
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.Request.Scope))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "scope is required"
            });
        }

        var scopes = request.Request.Scope.Split(' ').ToList();
        if (!scopes.Contains(ScopeTypes.OpenId))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = "openid scope is mandatory for backchannel authentication"
            });
        }

        if (!client.HasScopes(scopes, out var unsupportedScopes))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = $"'{string.Join(", ", unsupportedScopes)}' scopes are not allowed for this client"
            });
        }

        var hintsCount =
            (string.IsNullOrWhiteSpace(request.Request.LoginHint) ? 0 : 1) +
            (string.IsNullOrWhiteSpace(request.Request.LoginTokenHint) ? 0 : 1) +
            (string.IsNullOrWhiteSpace(request.Request.IdTokenHint) ? 0 : 1);

        if (hintsCount == 0)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "One of login_hint, login_token_hint or id_token_hint must be provided"
            });
        }

        if (hintsCount > 1)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "Multiple hints are not allowed"
            });
        }

        UserHint? hint = request.Request switch
        {
            { IdTokenHint: not null and not "" } => UserHint.IdTokenHint,
            { LoginTokenHint: not null and not "" } => UserHint.LoginTokenHint,
            { LoginHint: not null and not "" } => UserHint.LoginHint,
            _ => null
        };

        if (hint is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "Invalid hint"
            });
        }

        var userResolver = _userResolverProvider.GetResolver(hint.Value);
        var resolveResult = await userResolver.ResolveAsync(request.Request, cancellationToken);
        if (!resolveResult.Succeeded)
        {
            return Results.BadRequest(resolveResult.GetError());
        }

        if (resolveResult.User is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.UnknownUserId,
                Description = "Unknown user"
            });
        }
        
        if (request.Request.RequestedExpiry is not null &&
            (request.Request.RequestedExpiry > _options.MaxRequestLifetime.TotalSeconds ||
             request.Request.RequestedExpiry < _options.MinRequestLifetime.TotalSeconds))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = $"requested_expiry must be between {_options.MinRequestLifetime} " +
                              $"and {_options.MaxRequestLifetime} seconds"
            });
        }
        
        var requestedExpiry = request.Request.RequestedExpiry.HasValue 
            ? TimeSpan.FromSeconds(request.Request.RequestedExpiry.Value) 
            : _options.DefaultRequestLifetime;

        if (!string.IsNullOrWhiteSpace(request.Request.BindingMessage) && request.Request.BindingMessage.Length > 255)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidBindingMessage,
                Description = "binding_message must not be longer then 255 characters"
            });
        }

        if (client.RequireUserCode && string.IsNullOrWhiteSpace(request.Request.UserCode))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.MissingUserCode,
                Description = "user_code is missing"
            });
        }

        if (!string.IsNullOrWhiteSpace(request.Request.UserCode) && 
            (request.Request.UserCode.Length > _options.UserCodeMaxLength ||
             request.Request.UserCode.Length < _options.UserCodeMinLength))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidUserCode,
                Description = $"user_code length must be between {_options.UserCodeMinLength} " +
                              $"and {_options.UserCodeMaxLength} characters"
            });
        }
        
        var cibaRequest = new CibaRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            AuthReqId = _keyFactory.Create(_options.AuthReqIdLength),
            ClientId = client.Id,
            UserId = resolveResult.User.Id,
            State = CibaRequestState.Pending,
            Interval = _options.Interval,
            Scope = request.Request.Scope,
            UserCode = request.Request.UserCode,
            AcrValues = request.Request.AcrValues,
            BindingMessage = request.Request.BindingMessage,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.Add(requestedExpiry),
        };

        var result = await _cibaRequestManager.CreateAsync(cibaRequest, cancellationToken);
        if (!result.Succeeded) return result;
        
        return Results.Ok(new BackchannelAuthenticationResponse()
        {
            AuthReqId = cibaRequest.AuthReqId,
            Interval = cibaRequest.Interval,
            ExpiresIn = (int)requestedExpiry.TotalSeconds,
        });
    }
}