using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmResetPasswordCommand(ConfirmResetPasswordRequest Request)
    : IRequest<Result>;

internal sealed class ConfirmResetPasswordCommandHandler(
    AppManager appManager,
    ILogger<ConfirmResetPasswordCommandHandler> logger)
    : IRequestHandler<ConfirmResetPasswordCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(ConfirmResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var resetResult =
            await appManager.SecurityManager.ResetPasswordAsync(user, request.Request.Code,
                request.Request.NewPassword);

        if (!resetResult.Succeeded)
        {
            return resetResult;
        }

        return Result.Success("Your password has been successfully reset.");
    }
}