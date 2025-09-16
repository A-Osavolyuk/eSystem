using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record CreatePasskeyCommand(CreatePasskeyRequest Request, HttpContext HttpContext) : IRequest<Result>;

public class CreatePasskeyCommandHandler(
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<CreatePasskeyCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(CreatePasskeyCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var challenge = CredentialGenerator.GenerateChallenge();
        var options = CredentialGenerator.CreateCreationOptions(user, 
            request.Request.DisplayName, challenge, identityOptions.Credentials);
        
        request.HttpContext.Session.SetString("webauthn_attestation_challenge", challenge);
        
        return Result.Success(options);
    }
}