using System.Security.Claims;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Core.Security.Authorization.OAuth.Constants;
using eSecurity.Server.Security.Authentication;
using eSecurity.Server.Security.Authentication.SignIn;
using eSecurity.Server.Security.Identity.SignUp;
using eSecurity.Server.Security.Identity.SignUp.Strategies;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.OAuth;

public sealed record HandleLoginCommand(
    AuthenticationResult AuthenticationResult,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

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
            return Results.InternalServerError(request.RemoteError);

        var email = request.AuthenticationResult.Principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        //TODO: Implement redirect to fallback page on error
        if (email is null) return Results.BadRequest("Email is not provider in credentials");

        var user = await _userManager.FindByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            var signUpStrategy = _signUpResolver.Resolve(SignUpType.OAuth);
            var signUpPayload = new OAuthSignUpPayload()
            {
                Type = linkedAccountType,
                Email = email,
                ReturnUri = request.ReturnUri!,
                Token = token,
                SessionId = Guid.Parse(sessionId),
            };
            
            var signUpResult = await signUpStrategy.ExecuteAsync(signUpPayload, cancellationToken);
            
            return !signUpResult.Succeeded 
                ? Results.Found(fallbackUri) 
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
        
        var strategy = _signInResolver.Resolve(SignInType.OAuth);
        return await strategy.ExecuteAsync(signInPayload, cancellationToken);
    }
}