namespace eShop.Auth.Api.Features.Auth.Commands;

internal sealed record ConfirmResetPasswordCommand(ConfirmResetPasswordRequest Request)
    : IRequest<Result<ConfirmResetPasswordResponse>>;

internal sealed class ConfirmResetPasswordCommandHandler(
    AppManager appManager,
    ILogger<ConfirmResetPasswordCommandHandler> logger)
    : IRequestHandler<ConfirmResetPasswordCommand, Result<ConfirmResetPasswordResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<ConfirmResetPasswordResponse>> Handle(ConfirmResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email {request.Request.Email}."));
        }

        var resetResult =
            await appManager.SecurityManager.ResetPasswordAsync(user, request.Request.Code,
                request.Request.NewPassword);

        if (!resetResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot reset password for user with email {request.Request.Email} " +
                $"due to server error: {resetResult.Errors.First().Description}."));
        }

        return new(new ConfirmResetPasswordResponse()
        {
            Message = "Your password has been successfully reset."
        });
    }
}