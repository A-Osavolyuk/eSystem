using eSystem.Auth.Api.Security.Authentication.TwoFactor.Authenticator;
using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Features.TwoFactor.Commands;

public record RegenerateQrCodeCommand(RegenerateQrCodeRequest Request) : IRequest<Result>;

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
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var secret = secretManager.Generate();
        var email = user.GetEmail(EmailType.Primary)!.Email;
        var qrCode = qrCodeFactory.Create(email, secret, QrCodeConfiguration.Issuer);
        return Result.Success(qrCode);
    }
}