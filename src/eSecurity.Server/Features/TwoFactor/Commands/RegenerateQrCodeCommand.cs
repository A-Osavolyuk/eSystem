using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record RegenerateQrCodeCommand : IRequest<Result>;
public class RegenerateQrCodeCommandHandler(
    IUserManager userManager,
    IQrCodeFactory qrCodeFactory,
    IEmailManager emailManager,
    ISecretManager secretManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<RegenerateQrCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IQrCodeFactory _qrCodeFactory = qrCodeFactory;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ISecretManager _secretManager = secretManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(RegenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
                if (subjectClaim is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid subject"
            });
        }
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

        var secret = _secretManager.Generate();
        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (email is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Email not found"
            });
        }
        
        var qrCode = _qrCodeFactory.Create(secret, email.Email, QrCodeConfiguration.Issuer);
        return Results.Success(SuccessCodes.Ok, qrCode);
    }
}