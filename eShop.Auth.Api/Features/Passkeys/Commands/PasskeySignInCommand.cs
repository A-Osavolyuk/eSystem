using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record PasskeySignInCommand(PasskeySignInRequest Request, HttpContext Context) : IRequest<Result>;

public class PasskeySignInCommandHandler(
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<PasskeySignInCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(PasskeySignInCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByUsernameAsync(request.Request.Username, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with name {request.Request.Username}.");

        var challenge = CredentialGenerator.GenerateChallenge();
        var options = CredentialGenerator.CreateRequestOptions(user, challenge, identityOptions.Credentials);

        request.Context.Session.SetString("webauthn_assertion_challenge", challenge);

        return Result.Success(options);
    }
}