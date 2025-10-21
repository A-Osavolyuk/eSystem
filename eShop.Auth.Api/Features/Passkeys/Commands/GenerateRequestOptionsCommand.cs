using eShop.Auth.Api.Security.Credentials.PublicKey;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record GenerateRequestOptionsCommand(GenerateRequestOptionsRequest Request) : IRequest<Result>;

public class GenerateRequestOptionsCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    IChallengeFactory challengeFactory,
    ICredentialFactory credentialFactory,
    IOptions<CredentialOptions> options) : IRequestHandler<GenerateRequestOptionsCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISessionStorage sessionStorage = sessionStorage;
    private readonly IChallengeFactory challengeFactory = challengeFactory;
    private readonly ICredentialFactory credentialFactory = credentialFactory;
    private readonly CredentialOptions credentialOptions = options.Value;
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

        var challenge = challengeFactory.Create();
        var options = credentialFactory.CreateRequestOptions(device.Passkey, challenge, credentialOptions);

        sessionStorage.Set(ChallengeSessionKeys.Assertion, challenge);
        return Result.Success(options);
    }
}