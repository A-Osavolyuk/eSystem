using System.Security.Claims;
using eSystem.Auth.Api.Security.Authentication;
using eSystem.Auth.Api.Security.Authentication.SignIn;
using eSystem.Auth.Api.Security.Authentication.SignIn.Strategies;
using eSystem.Auth.Api.Security.Authorization.OAuth;
using eSystem.Auth.Api.Security.Identity.SignUp;
using eSystem.Auth.Api.Security.Identity.SignUp.Strategies;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.Security.Authentication.SignIn;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSystem.Auth.Api.Features.LinkedAccounts.Commands;

public sealed record HandleLoginCommand(
    AuthenticationResult AuthenticationResult,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

public sealed class HandleOAuthLoginCommandHandler(
    IUserManager userManager,
    ISignUpResolver signUpResolver,
    ISignInResolver signInResolver) : IRequestHandler<HandleLoginCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISignUpResolver signUpResolver = signUpResolver;
    private readonly ISignInResolver signInResolver = signInResolver;

    public async Task<Result> Handle(HandleLoginCommand request,
        CancellationToken cancellationToken)
    {
        var authenticationResult = request.AuthenticationResult;
        var items = authenticationResult.Properties.Items;
        var linkedAccountName = request.AuthenticationResult.Principal.Identity!.AuthenticationType!;
        var fallbackUri = items["fallbackUri"]!;
        var sessionId = items["sessionId"]!;
        var token = items["token"]!;

        var linkedAccountType = linkedAccountName switch
        {
            AuthenticationTypes.Google => LinkedAccountType.Google,
            AuthenticationTypes.Facebook => LinkedAccountType.Facebook,
            AuthenticationTypes.Microsoft => LinkedAccountType.Microsoft,
            AuthenticationTypes.X or AuthenticationTypes.Twitter => LinkedAccountType.X,
            _ => throw new NotSupportedException("Unknown linked account type")
         };

        if (!string.IsNullOrEmpty(request.RemoteError)) 
            return Results.InternalServerError(request.RemoteError, fallbackUri);

        var email = request.AuthenticationResult.Principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null) return Results.BadRequest("Email is not provider in credentials", fallbackUri);

        var user = await userManager.FindByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            var signUpStrategy = signUpResolver.Resolve(SignUpType.OAuth);
            var signUpPayload = new OAuthSignUpPayload()
            {
                Type = linkedAccountType,
                Email = email,
                ReturnUri = request.ReturnUri!,
                Token = token,
                SessionId = Guid.Parse(sessionId),
            };
            
            var signUpResult = await signUpStrategy.SignUpAsync(signUpPayload, cancellationToken);
            
            return !signUpResult.Succeeded 
                ? Result.Failure(signUpResult.GetError(), fallbackUri) 
                : signUpResult;
        }

        var signInPayload = new OAuthSignInPayload()
        {
            LinkedAccount = linkedAccountType,
            Email = email,
            ReturnUri = request.ReturnUri!,
            Token = token,
            SessionId = Guid.Parse(sessionId),
        };
        
        var strategy = signInResolver.Resolve(SignInType.OAuth);
        return await strategy.SignInAsync(signInPayload, cancellationToken);
    }
}