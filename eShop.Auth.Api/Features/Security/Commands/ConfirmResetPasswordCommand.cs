using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmResetPasswordCommand(ConfirmResetPasswordRequest Request)
    : IRequest<Result>;

internal sealed class ConfirmResetPasswordCommandHandler(
    ISecurityManager securityManager,
    UserManager<UserEntity> userManager,
    ILogger<ConfirmResetPasswordCommandHandler> logger)
    : IRequestHandler<ConfirmResetPasswordCommand, Result>
{
    private readonly ISecurityManager securityManager = securityManager;
    private readonly UserManager<UserEntity> userManager = userManager;

    public async Task<Result> Handle(ConfirmResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var resetResult = await securityManager.ResetPasswordAsync(user, request.Request.Code, request.Request.NewPassword);

        return !resetResult.Succeeded ? resetResult : Result.Success("Your password has been successfully reset.");
    }
}