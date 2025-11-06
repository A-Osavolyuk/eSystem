using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Security.Authentication.TwoFactor.Secret;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Identity.Email;

namespace eSecurity.Features.TwoFactor.Commands;

public record GenerateQrCodeCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
}

public class GenerateQrCodeCommandHandler(
    IUserManager userManager,
    IQrCodeFactory qrCodeFactory,
    ISecretManager secretManager) : IRequestHandler<GenerateQrCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IQrCodeFactory qrCodeFactory = qrCodeFactory;
    private readonly ISecretManager secretManager = secretManager;

    public async Task<Result> Handle(GenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var email = user.GetEmail(EmailType.Primary)?.Email!;

        var userSecret = user.Secret;
        if (userSecret is null)
        {
            var secret = secretManager.Generate();
            var protectedSecret = secretManager.Protect(secret);
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

        var unprotectedSecret = secretManager.Unprotect(userSecret.Secret);
        var qrCode = qrCodeFactory.Create(email, unprotectedSecret, QrCodeConfiguration.Issuer);

        return Result.Success(qrCode);
    }
}