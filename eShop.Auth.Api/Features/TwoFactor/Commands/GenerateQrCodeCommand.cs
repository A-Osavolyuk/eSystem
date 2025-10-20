using eShop.Auth.Api.Security.Constants;
using eShop.Domain.DTOs;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record GenerateQrCodeCommand(GenerateQrCodeRequest Request) : IRequest<Result>;

public class GenerateQrCodeCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager) : IRequestHandler<GenerateQrCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
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
            
            var secretResult = await secretManager.SaveAsync(userSecret, cancellationToken);
            if (!secretResult.Succeeded) return secretResult;
        }

        var unprotectedSecret = secretManager.Unprotect(userSecret.Secret);
        var qrCode = QrCodeGenerator.Generate(email, unprotectedSecret, QrCodeConfiguration.Issuer);
        var response = new QrCode()
        {
            Value = qrCode,
            Secret = unprotectedSecret
        };

        return Result.Success(response);
    }
}