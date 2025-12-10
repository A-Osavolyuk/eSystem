using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record GenerateQrCodeCommand(GenerateQrCodeRequest Request) : IRequest<Result>;

public class GenerateQrCodeCommandHandler(
    IUserManager userManager,
    IQrCodeFactory qrCodeFactory,
    ISecretManager secretManager,
    IEmailManager emailManager,
    IDataProtectionProvider protectionProvider) : IRequestHandler<GenerateQrCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IQrCodeFactory _qrCodeFactory = qrCodeFactory;
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;

    public async Task<Result> Handle(GenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (email is null) return Results.NotFound("Email not found");

        var userSecret = await _secretManager.GetAsync(user, cancellationToken);
        if (userSecret is null)
        {
            var secret = _secretManager.Generate();
            var protectedSecret = protector.Protect(secret);
            userSecret = new UserSecretEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Secret = protectedSecret,
                CreateDate = DateTimeOffset.UtcNow,
            };
            
            var secretResult = await _secretManager.AddAsync(userSecret, cancellationToken);
            if (!secretResult.Succeeded) return secretResult;
        }

        var unprotectedSecret = protector.Unprotect(userSecret.Secret);
        var qrCode = _qrCodeFactory.Create(unprotectedSecret, email.Email, QrCodeConfiguration.Issuer);

        return Results.Ok(qrCode);
    }
}