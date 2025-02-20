namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangePasswordCommand(ChangePasswordRequest Request)
    : IRequest<Result<ChangePasswordResponse>>;

internal sealed class ChangePasswordCommandHandler(
    AppManager appManager)
    : IRequestHandler<ChangePasswordCommand, Result<ChangePasswordResponse>>
{
    private readonly AppManager appManager = appManager;

    async Task<Result<ChangePasswordResponse>>
        IRequestHandler<ChangePasswordCommand, Result<ChangePasswordResponse>>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);
        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email {request.Request.Email}."));
        }

        var isCorrectPassword =
            await appManager.UserManager.CheckPasswordAsync(user, request.Request.OldPassword);
        if (!isCorrectPassword)
        {
            return new(new BadRequestException($"Wrong password."));
        }

        var result = await appManager.UserManager.ChangePasswordAsync(user, request.Request.OldPassword,
            request.Request.NewPassword);
        if (!result.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot change password due to server error: {result.Errors.First().Description}."));
        }

        return new(new ChangePasswordResponse() { Message = "Password has been successfully changed." });
    }
}