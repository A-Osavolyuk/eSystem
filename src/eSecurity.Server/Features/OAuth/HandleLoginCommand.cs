using System.Security.Claims;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Core.Security.Authorization.OAuth.Constants;
using eSecurity.Server.Security.Authentication;
using eSecurity.Server.Security.Authentication.SignIn;
using eSecurity.Server.Security.Identity.SignUp;
using eSecurity.Server.Security.Identity.SignUp.Strategies;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Server.Features.OAuth;

public sealed record HandleLoginCommand(
    AuthenticationResult AuthenticationResult,
    string? RemoteError,
    string ReturnUri) : IRequest<Result>;

public sealed class HandleOAuthLoginCommandHandler(
    IUserManager userManager,
    ISignUpResolver signUpResolver,
    ISignInResolver signInResolver) : IRequestHandler<HandleLoginCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ISignUpResolver _signUpResolver = signUpResolver;
    private readonly ISignInResolver _signInResolver = signInResolver;

    public async Task<Result> Handle(HandleLoginCommand request,
        CancellationToken cancellationToken)
    {
        var authenticationResult = request.AuthenticationResult;
        var items = authenticationResult.Properties.Items;
        var provider = request.AuthenticationResult.Principal.Identity!.AuthenticationType!;
        var sessionId = items["sid"]!;
        var state = items["state"]!;

        var linkedAccountType = provider switch
        {
            AuthenticationTypes.Google => LinkedAccountType.Google,
            AuthenticationTypes.Facebook => LinkedAccountType.Facebook,
            AuthenticationTypes.Microsoft => LinkedAccountType.Microsoft,
            AuthenticationTypes.X or AuthenticationTypes.Twitter => LinkedAccountType.X,
            _ => throw new NotSupportedException("Unknown linked account type")
         };

        if (!string.IsNullOrEmpty(request.RemoteError))
        {
            return Results.Found(QueryBuilder.Create()
                .WithUri(request.ReturnUri)
                .WithQueryParam("error", Errors.OAuth.ServerError)
                .WithQueryParam("error_description", request.RemoteError)
                .Build());
        }

        var email = request.AuthenticationResult.Principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null)
        {
            return Results.Found(QueryBuilder.Create()
                .WithUri(request.ReturnUri)
                .WithQueryParam("error", Errors.Common.InvalidCredentials)
                .WithQueryParam("error_description", "Email was not provided in credentials.")
                .Build());
        }

        var user = await _userManager.FindByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            var signUpStrategy = _signUpResolver.Resolve(SignUpType.OAuth);
            var signUpPayload = new OAuthSignUpPayload()
            {
                Type = linkedAccountType,
                Email = email,
                ReturnUri = request.ReturnUri,
                State = state,
                SessionId = Guid.Parse(sessionId),
            };
            
            var signUpResult = await signUpStrategy.ExecuteAsync(signUpPayload, cancellationToken);
            if (signUpResult.Succeeded) return signUpResult;
            
            var signUpError = signUpResult.GetError();
            return Results.Found(QueryBuilder.Create()
                .WithUri(request.ReturnUri)
                .WithQueryParam("error", signUpError.Code)
                .WithQueryParam("error_description", signUpError.Description)
                .Build());

        }

        var signInPayload = new OAuthSignInPayload()
        {
            Email = email,
            State = state,
            LinkedAccount = linkedAccountType,
            ReturnUri = request.ReturnUri!,
            SessionId = Guid.Parse(sessionId),
        };
        
        var strategy = _signInResolver.Resolve(SignInType.OAuth);
        var signInResult = await strategy.ExecuteAsync(signInPayload, cancellationToken);
        if (signInResult.Succeeded) return signInResult;
        
        var signInError = signInResult.GetError();
        return Results.Found(QueryBuilder.Create()
            .WithUri(request.ReturnUri)
            .WithQueryParam("error", signInError.Code)
            .WithQueryParam("error_description", signInError.Description)
            .Build());
    }
}