using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record GenerateCreationOptionsCommand(GenerateCreationOptionsRequest Request) : IRequest<Result>;

public class GenerateCreationOptionsCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    IdentityOptions identityOptions) : IRequestHandler<GenerateCreationOptionsCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IdentityOptions identityOptions = identityOptions;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateCreationOptionsCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = httpContext.GetUserAgent();
        var ip = httpContext.GetIpV4();

        var device = await deviceManager.FindAsync(user, userAgent, ip, cancellationToken);
        if (device is null) return Results.BadRequest("Invalid device.");

        var challenge = CredentialGenerator.GenerateChallenge();
        var displayName = request.Request.DisplayName;
        var browser = device.Browser!.Split(" ").First();
        var fingerprint = $"{device.Device}_{browser}";
        
        var options = CredentialGenerator.CreateCreationOptions(user,
            displayName, challenge, fingerprint, identityOptions.Credentials);

        httpContext.Session.SetString(ChallengeSessionKeys.Attestation, challenge);

        return Result.Success(options);
    }
}