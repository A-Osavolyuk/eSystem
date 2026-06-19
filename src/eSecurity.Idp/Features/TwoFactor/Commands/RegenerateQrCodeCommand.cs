using eSecurity.Idp.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Cryptography;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.TwoFactor.Commands;

public record RegenerateQrCodeCommand : IRequest<Result>;
public class RegenerateQrCodeCommandHandler(
    IQrCodeFactory qrCodeFactory,
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService) : IRequestHandler<RegenerateQrCodeCommand, Result>
{
    private readonly IQrCodeFactory _qrCodeFactory = qrCodeFactory;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;

    public async Task<Result> Handle(RegenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
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
        var email = await _emailQueryService.GetByTypeAsync(user.Id, EmailType.Primary, cancellationToken);
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