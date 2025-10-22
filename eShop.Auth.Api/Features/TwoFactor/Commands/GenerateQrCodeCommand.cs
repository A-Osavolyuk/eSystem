using eShop.Auth.Api.Security.Authentication.TwoFactor.Authenticator;
using eShop.Domain.Common.Results;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record GenerateQrCodeCommand(GenerateQrCodeRequest Request) : IRequest<Result>;

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
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
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