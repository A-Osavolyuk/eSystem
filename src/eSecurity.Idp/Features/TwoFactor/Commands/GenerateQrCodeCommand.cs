using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Idp.Security.Authentication.TwoFactor.Secret;
using eSecurity.Idp.Security.Cryptography.Protection.Constants;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Cryptography;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Idp.Features.TwoFactor.Commands;

public record GenerateQrCodeCommand() : IRequest<Result>;

public class GenerateQrCodeCommandHandler(
    IUserQueryService userQueryService,
    ICurrentUserAccessor currentUserAccessor,
    IQrCodeFactory qrCodeFactory,
    ISecretManager secretManager,
    IEmailQueryService emailQueryService,
    IDataProtectionProvider protectionProvider) : IRequestHandler<GenerateQrCodeCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IQrCodeFactory _qrCodeFactory = qrCodeFactory;
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;

    public async Task<Result> Handle(GenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        
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

        var email = await _emailQueryService.GetByTypeAsync(user.Id, EmailType.Primary, cancellationToken);
        if (email is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Email not found"
            });
        }

        var userSecret = await _secretManager.GetAsync(user, cancellationToken);
        if (userSecret is null)
        {
            var secret = RandomKeyFactory.Create(20);
            var protectedSecret = protector.Protect(secret);
            userSecret = new UserSecretEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ProtectedSecret = protectedSecret
            };
            
            var secretResult = await _secretManager.AddAsync(userSecret, cancellationToken);
            if (!secretResult.Succeeded) return secretResult;
        }

        var unprotectedSecret = protector.Unprotect(userSecret.ProtectedSecret);
        var qrCode = _qrCodeFactory.Create(unprotectedSecret, email.Email, QrCodeConfiguration.Issuer);

        return Results.Success(SuccessCodes.Ok, qrCode);
    }
}