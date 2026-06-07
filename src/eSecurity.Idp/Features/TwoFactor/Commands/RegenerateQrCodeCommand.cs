using eSecurity.Idp.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Idp.Security.Authentication.TwoFactor.Secret;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Cryptography;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.TwoFactor.Commands;

public record RegenerateQrCodeCommand : IRequest<Result>;
public class RegenerateQrCodeCommandHandler(
    IUserManager userManager,
    IQrCodeFactory qrCodeFactory,
    IEmailManager emailManager) : IRequestHandler<RegenerateQrCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IQrCodeFactory _qrCodeFactory = qrCodeFactory;
    private readonly IEmailManager _emailManager = emailManager;

    public async Task<Result> Handle(RegenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userManager.GetUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var secret = RandomKeyFactory.Create(20);
        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (email is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Email not found"
            });
        }
        
        var qrCode = _qrCodeFactory.Create(secret, email.Email, QrCodeConfiguration.Issuer);
        return Results.Success(SuccessCodes.Ok, qrCode);
    }
}