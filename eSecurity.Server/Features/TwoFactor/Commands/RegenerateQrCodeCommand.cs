using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record RegenerateQrCodeCommand(RegenerateQrCodeRequest Request) : IRequest<Result>;
public class RegenerateQrCodeCommandHandler(
    IUserManager userManager,
    IQrCodeFactory qrCodeFactory,
    ISecretManager secretManager) : IRequestHandler<RegenerateQrCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IQrCodeFactory _qrCodeFactory = qrCodeFactory;
    private readonly ISecretManager _secretManager = secretManager;

    public async Task<Result> Handle(RegenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var secret = _secretManager.Generate();
        var email = user.GetEmail(EmailType.Primary)!.Email;
        var qrCode = _qrCodeFactory.Create(email, secret, QrCodeConfiguration.Issuer);
        return Result.Success(qrCode);
    }
}