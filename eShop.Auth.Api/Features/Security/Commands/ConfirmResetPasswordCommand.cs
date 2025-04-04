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
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var resetResult =
            await appManager.SecurityManager.ResetPasswordAsync(user, request.Request.Code,
                request.Request.NewPassword);

        if (!resetResult.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Internal server error",
                Details = $"Cannot reset password for user with email {request.Request.Email} " +
                          $"due to server error: {resetResult.Errors.First().Description}."
            });
        }

        return Result.Success("Your password has been successfully reset.");
    }
}