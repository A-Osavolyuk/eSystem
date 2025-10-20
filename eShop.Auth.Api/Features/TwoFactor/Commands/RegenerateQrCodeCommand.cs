using eShop.Auth.Api.Security.Constants;
using eShop.Domain.DTOs;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record RegenerateQrCodeCommand(RegenerateQrCodeRequest Request) : IRequest<Result>;

public class RegenerateQrCodeCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager) : IRequestHandler<RegenerateQrCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISecretManager secretManager = secretManager;

    public async Task<Result> Handle(RegenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var secret = secretManager.Generate();
        var email = user.GetEmail(EmailType.Primary)!.Email;
        var qrCode = QrCodeGenerator.Generate(email, secret, QrCodeConfiguration.Issuer);
        var response = new QrCode()
        {
            Value = qrCode,
            Secret = secret
        };

        return Result.Success(response);
    }
}