using eShop.Auth.Api.Security.Protection;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record GenerateQrCodeCommand(GenerateQrCodeRequest Request) : IRequest<Result>;

public class GenerateQrCodeCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager,
    SecretProtector protector) : IRequestHandler<GenerateQrCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly SecretProtector protector = protector;
    private const string QrCodeIssuer = "eShop";

    public async Task<Result> Handle(GenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var secret = await secretManager.FindAsync(user, cancellationToken);
        
        if (secret is null)
        {
            secret = await secretManager.GenerateAsync(user, cancellationToken);
        }
        
        var unprotectedSecret = protector.Unprotect(secret.Secret);
        var qrCode = QrCodeGenerator.Generate(user.Email, unprotectedSecret, QrCodeIssuer);
            
        var response = new GenerateQrCodeResponse() { QrCode = qrCode };
            
        return Result.Success(response);
    }
}