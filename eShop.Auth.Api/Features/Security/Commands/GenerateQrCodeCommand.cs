using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record GenerateQrCodeCommand(GenerateQrCodeRequest Request) : IRequest<Result>;

public class GenerateQrCodeCommandHandler(
    ITwoFactorManager twoFactorManager,
    IUserManager userManager) : IRequestHandler<GenerateQrCodeCommand, Result>
{
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GenerateQrCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound("User not found");
        }

        var qrCode = await twoFactorManager.GenerateQrCodeAsync(user, cancellationToken);
        
        var response = new GenerateQrCodeResponse(){ QrCode = qrCode };
        
        return Result.Success(response);
    }
}