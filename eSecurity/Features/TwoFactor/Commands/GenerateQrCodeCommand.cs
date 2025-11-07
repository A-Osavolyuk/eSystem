using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Security.Authentication.TwoFactor.Secret;
using eSecurity.Security.Cryptography.Protection;
using eSecurity.Security.Identity;
using eSecurity.Security.Identity.User;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Features.TwoFactor.Commands;

public record GenerateQrCodeCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
}

public class GenerateQrCodeCommandHandler(
    IUserManager userManager,
    IQrCodeFactory qrCodeFactory,
    ISecretManager secretManager,
    IDataProtectionProvider protectionProvider) : IRequestHandler<GenerateQrCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IQrCodeFactory qrCodeFactory = qrCodeFactory;
    private readonly ISecretManager secretManager = secretManager;
    private readonly IDataProtectionProvider protectionProvider = protectionProvider;

    public async Task<Result> Handle(GenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var protector = protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var email = user.GetEmail(EmailType.Primary)?.Email!;

        var userSecret = user.Secret;
        if (userSecret is null)
        {
            var secret = secretManager.Generate();


            var protectedSecret = protector.Protect(secret);
            userSecret = new UserSecretEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Secret = protectedSecret,
                CreateDate = DateTimeOffset.UtcNow,
            };
            
            var secretResult = await secretManager.AddAsync(userSecret, cancellationToken);
            if (!secretResult.Succeeded) return secretResult;
        }

        var unprotectedSecret = protector.Unprotect(userSecret.Secret);
        var qrCode = qrCodeFactory.Create(email, unprotectedSecret, QrCodeConfiguration.Issuer);

        return Result.Success(qrCode);
    }
}