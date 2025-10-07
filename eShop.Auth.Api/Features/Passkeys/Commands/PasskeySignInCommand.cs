using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record PasskeySignInCommand(PasskeySignInRequest Request) : IRequest<Result>;

public class PasskeySignInCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    IdentityOptions identityOptions) : IRequestHandler<PasskeySignInCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(PasskeySignInCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByUsernameAsync(request.Request.Username, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with name {request.Request.Username}.");

        var challenge = CredentialGenerator.GenerateChallenge();
        var options = CredentialGenerator.CreateRequestOptions(user, challenge, identityOptions.Credentials);

        httpContext.Session.SetString(ChallengeSessionKeys.Assertion, challenge);

        return Result.Success(options);
    }
}