using eSystem.Auth.Api.Security.Credentials.PublicKey;
using eSystem.Auth.Api.Security.Credentials.PublicKey.Challenge;
using eSystem.Auth.Api.Security.Credentials.PublicKey.Credentials;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Auth.Api.Storage.Session;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Credentials.Constants;

namespace eSystem.Auth.Api.Features.Passkeys.Commands;

public record GenerateCreationOptionsCommand(GenerateCreationOptionsRequest Request) : IRequest<Result>;

public class GenerateCreationOptionsCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    ICredentialFactory credentialFactory,
    IChallengeFactory challengeFactory,
    IOptions<CredentialOptions> options) : IRequestHandler<GenerateCreationOptionsCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISessionStorage sessionStorage = sessionStorage;
    private readonly ICredentialFactory credentialFactory = credentialFactory;
    private readonly IChallengeFactory challengeFactory = challengeFactory;
    private readonly CredentialOptions credentialOptions = options.Value;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateCreationOptionsCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = httpContext.GetUserAgent();
        var ipAddress = httpContext.GetIpV4();
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.BadRequest("Invalid device.");

        var challenge = challengeFactory.Create();
        var displayName = request.Request.DisplayName;
        var browser = device.Browser!.Split(" ").First();
        var fingerprint = $"{device.Device}_{browser}";
        
        var options = credentialFactory.CreateCreationOptions(user,
            displayName, challenge, fingerprint, credentialOptions);

        sessionStorage.Set(ChallengeSessionKeys.Attestation, challenge);
        return Result.Success(options);
    }
}