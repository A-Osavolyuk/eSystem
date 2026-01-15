using eSecurity.Core.Security.Authentication.SignIn.Session;
using eSecurity.Server.Common.Responses;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.SignIn.Session;
using eSecurity.Server.Security.Identity.Options;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Utilities.Query;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Features.OAuth;

public sealed record OAuthLoginCommand(string Provider, string ReturnUri, string State) : IRequest<Result>;

public sealed class OAuthLoginCommandHandler(
    ISignInSessionManager signInSessionManager,
    IOptions<SignInOptions> options) : IRequestHandler<OAuthLoginCommand, Result>
{
    private readonly ISignInSessionManager _signInSessionManager = signInSessionManager;
    private readonly SignInOptions _options = options.Value;

    public async Task<Result> Handle(OAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        if (!_options.AllowOAuthLogin)
        {
            return Results.InternalServerError(QueryBuilder.Create()
                .WithUri(request.ReturnUri)
                .WithQueryParam("error", ErrorTypes.OAuth.ServerError)
                .WithQueryParam("error_description", "OAuth is not allowed")
                .Build());
        }
        
        var session = new SignInSessionEntity()
        {
            Id = Guid.NewGuid(),
            Provider = request.Provider,
            CurrentStep = SignInStep.OAuth,
            RequiredSteps = [SignInStep.OAuth],
            Status = SignInStatus.InProgress,
            StartDate = DateTimeOffset.UtcNow,
            ExpireDate = DateTimeOffset.UtcNow.AddMinutes(15),
        };
        
        var builder = QueryBuilder.Create()
            .WithUri("/api/v1/oauth/handle")
            .WithQueryParam("returnUri", request.ReturnUri);
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = builder.Build(),
            Items = 
            {
                { "sid", session.Id.ToString() },
                { "state", request.State },
            }
        };
        
        await _signInSessionManager.CreateAsync(session, cancellationToken);
        
        var result = Results.Ok(new OAuthLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
        
        return result;
    }
}