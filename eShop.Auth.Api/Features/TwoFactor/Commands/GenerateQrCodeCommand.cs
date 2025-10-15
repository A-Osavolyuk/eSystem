using eShop.Auth.Api.Security.Protection;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record GenerateQrCodeCommand(GenerateQrCodeRequest Request) : IRequest<Result>;

public class GenerateQrCodeCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager,
    SecretProtector protector) : IRequestHandler<GenerateQrCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly SecretProtector protector = protector;
    private const string QrCodeIssuer = "eAccount";

    public async Task<Result> Handle(GenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var secret = await secretManager.FindAsync(user, cancellationToken);
        if (secret is null) secret = await secretManager.GenerateAsync(user, cancellationToken);
        
        var unprotectedSecret = protector.Unprotect(secret.Secret);
        var email = user.GetEmail(EmailType.Primary)?.Email!;
        
        var qrCode = QrCodeGenerator.Generate(email, unprotectedSecret, QrCodeIssuer);
            
        var response = new GenerateQrCodeResponse()
        {
            QrCode = qrCode,
            Secret = secret.Secret
        };
            
        return Result.Success(response);
    }
}