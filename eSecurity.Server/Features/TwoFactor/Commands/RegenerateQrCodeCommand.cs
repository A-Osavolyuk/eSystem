using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record RegenerateQrCodeCommand(RegenerateQrCodeRequest Request) : IRequest<Result>;
public class RegenerateQrCodeCommandHandler(
    IUserManager userManager,
    IQrCodeFactory qrCodeFactory,
    IEmailManager emailManager,
    ISecretManager secretManager) : IRequestHandler<RegenerateQrCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IQrCodeFactory _qrCodeFactory = qrCodeFactory;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ISecretManager _secretManager = secretManager;

    public async Task<Result> Handle(RegenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var secret = _secretManager.Generate();
        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (email is null) return Results.NotFound("Email not found");
        
        var qrCode = _qrCodeFactory.Create(secret, email.Email, QrCodeConfiguration.Issuer);
        return Results.Ok(qrCode);
    }
}