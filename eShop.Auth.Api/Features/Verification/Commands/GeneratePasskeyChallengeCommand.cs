using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Verification.Commands;

public record GeneratePasskeyChallengeCommand(GeneratePasskeyChallengeRequest Request) : IRequest<Result>;

public class GeneratePasskeyChallengeCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor contextAccessor,
    IdentityOptions identityOptions) : IRequestHandler<GeneratePasskeyChallengeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;
    private readonly HttpContext httpContext = contextAccessor.HttpContext!;

    public async Task<Result> Handle(GeneratePasskeyChallengeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var challenge = CredentialGenerator.GenerateChallenge();
        var options = CredentialGenerator.CreateRequestOptions(user, challenge, identityOptions.Credentials);

        httpContext.Session.SetString(ChallengeSessionKeys.Verification, challenge);

        return Result.Success(options);
    }
}