using eSecurity.Core.Requests;
using eSecurity.Idp.Security.Authentication.EndSession;
using eSecurity.Idp.Security.Authentication.EndSession.Backchannel;
using eSecurity.Idp.Security.Authentication.EndSession.Frontchannel;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Features.Connect.Commands;

public sealed record ConfirmEndSessionCommand(ConfirmEndSessionRequest Request) : IRequest<Result>;

public sealed class ConfirmEndSessionCommandHandler(
    IEndSessionManager endSessionManager,
    IOptions<EndSessionOptions> options,
    IBackchannelLogoutHandler backchannelLogoutHandler,
    IFrontChannelLogoutHandler frontChannelLogoutHandler,
    ISessionManager sessionManager,
    ISessionAccessor sessionAccessor) : IRequestHandler<ConfirmEndSessionCommand, Result>
{
    private readonly IEndSessionManager _endSessionManager = endSessionManager;
    private readonly IBackchannelLogoutHandler _backchannelLogoutHandler = backchannelLogoutHandler;
    private readonly IFrontChannelLogoutHandler _frontChannelLogoutHandler = frontChannelLogoutHandler;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly EndSessionOptions _options = options.Value;

    public async Task<Result> Handle(ConfirmEndSessionCommand request, CancellationToken cancellationToken = default)
    {
        var redirectUri = _options.FallbackUrl;
        var loggedOutUri = _options.LoggedOutUrl;

        if (string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(loggedOutUri))
            throw new InvalidOperationException("End session flow was not configured correctly");

        var endSessionRequest = await _endSessionManager.FindByIdAsync(request.Request.RequestId, cancellationToken);
        if (endSessionRequest is null)
        {
            return Fallback(redirectUri, ErrorCode.InvalidRequest,
                "end_session_request_uri is invalid", request.Request.State);
        }

        if (endSessionRequest.Status is not EndSessionStatus.Pending)
        {
            return Fallback(redirectUri, ErrorCode.InvalidRequest,
                "End session request is invalid. Please, try again later", request.Request.State);
        }

        if (endSessionRequest.ExpiredAt > DateTimeOffset.UtcNow)
        {
            endSessionRequest.Status = EndSessionStatus.Expired;

            var updateResult = await _endSessionManager.UpdateAsync(endSessionRequest, cancellationToken);
            if (!updateResult.Succeeded)
            {
                var error = updateResult.GetError();
                return Fallback(redirectUri, error.Code, error.Description, endSessionRequest.State);
            }

            return Fallback(redirectUri, ErrorCode.InvalidRequest,
                "End session request is already expired", endSessionRequest.State);
        }

        if (!string.IsNullOrEmpty(endSessionRequest.State))
        {
            if (string.IsNullOrEmpty(request.Request.State) || !endSessionRequest.State.Equals(request.Request.State))
            {
                return Fallback(redirectUri, ErrorCode.InvalidRequest,
                    "state is invalid", endSessionRequest.State);
            }
        }
        else if (!string.IsNullOrEmpty(request.Request.State))
        {
            return Fallback(redirectUri, ErrorCode.InvalidRequest,
                "state is invalid", endSessionRequest.State);
        }
        
        endSessionRequest.Status = EndSessionStatus.Approved;

        var result = await _endSessionManager.UpdateAsync(endSessionRequest, cancellationToken);
        if (!result.Succeeded)
        {
            var error = result.GetError();
            return Fallback(redirectUri, error.Code, error.Description, endSessionRequest.State);
        }

        if (endSessionRequest.Client.AllowBackChannelLogout)
        {
            var backchannelResult = await _backchannelLogoutHandler.HandleAsync(endSessionRequest, cancellationToken);
            if (!backchannelResult.Succeeded)
            {
                var error = backchannelResult.GetError();
                return Fallback(redirectUri, error.Code, error.Description, endSessionRequest.State);
            }
        }

        if (endSessionRequest.Client.AllowFrontChannelLogout)
        {
            var frontchannelResult = await _frontChannelLogoutHandler.HandleAsync(endSessionRequest, cancellationToken);
            if (!frontchannelResult.Succeeded)
            {
                var error = frontchannelResult.GetError();
                return Fallback(redirectUri, error.Code, error.Description, endSessionRequest.State);
            }

            return frontchannelResult;
        }

        var sessionResult = await _sessionManager.RemoveAsync(endSessionRequest.Session, cancellationToken);
        if (!sessionResult.Succeeded)
        {
            var error = sessionResult.GetError();
            return Fallback(redirectUri, error.Code, error.Description, endSessionRequest.State);
        }

        _sessionAccessor.Remove();

        var postLogoutRedirectUri = endSessionRequest.PostLogoutRedirectUri;
        if (string.IsNullOrEmpty(postLogoutRedirectUri))
            postLogoutRedirectUri = loggedOutUri;

        var builder = QueryBuilder.Create().WithUri(postLogoutRedirectUri);
        if (!string.IsNullOrEmpty(endSessionRequest.State))
            builder.WithQueryParam("state", endSessionRequest.State);

        return Results.Redirect(RedirectionCode.Found, builder.Build());
    }

    private static Result Fallback(string uri, ErrorCode error, string description, string? state = null)
    {
        var builder = QueryBuilder.Create()
            .WithUri(uri)
            .WithQueryParam("error", error)
            .WithQueryParam("error_description", description);

        if (!string.IsNullOrEmpty(state))
            builder.WithQueryParam("state", state);

        return Results.Redirect(RedirectionCode.Found, builder.Build());
    }
}