using eSecurity.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Security.Authentication.TwoFactor.Secret;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Identity.Email;

namespace eSecurity.Features.TwoFactor.Commands;

public record RegenerateQrCodeCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
}

public class RegenerateQrCodeCommandHandler(
    IUserManager userManager,
    IQrCodeFactory qrCodeFactory,
    ISecretManager secretManager) : IRequestHandler<RegenerateQrCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IQrCodeFactory qrCodeFactory = qrCodeFactory;
    private readonly ISecretManager secretManager = secretManager;

    public async Task<Result> Handle(RegenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var secret = secretManager.Generate();
        var email = user.GetEmail(EmailType.Primary)!.Email;
        var qrCode = qrCodeFactory.Create(email, secret, QrCodeConfiguration.Issuer);
        return Result.Success(qrCode);
    }
}