using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record GenerateRequestOptionsCommand(GenerateRequestOptionsRequest Request) : IRequest<Result>;

public class GenerateRequestOptionsCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    IdentityOptions identityOptions) : IRequestHandler<GenerateRequestOptionsCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISessionStorage sessionStorage = sessionStorage;
    private readonly IdentityOptions identityOptions = identityOptions;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateRequestOptionsCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByUsernameAsync(request.Request.Username, cancellationToken);

        if (user is null)
        {
            user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
            if (user is null) return Results.NotFound("Cannot find user.");
        }

        var userAgent = httpContext.GetUserAgent();
        var ipAddress = httpContext.GetIpV4();
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.BadRequest("Invalid device.");
        if (device.Passkey is null) return Results.BadRequest("This device does not have a passkey.");

        var challenge = CredentialGenerator.GenerateChallenge();
        var options = CredentialGenerator.CreateRequestOptions(device.Passkey, challenge, identityOptions.Credentials);

        sessionStorage.Set(ChallengeSessionKeys.Assertion, challenge);
        return Result.Success(options);
    }
}