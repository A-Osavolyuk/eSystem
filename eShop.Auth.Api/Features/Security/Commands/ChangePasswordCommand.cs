using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangePasswordCommand(ChangePasswordRequest Request)
    : IRequest<Result>;

internal sealed class ChangePasswordCommandHandler(
    UserManager<UserEntity> userManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly UserManager<UserEntity> userManager = userManager;

    async Task<Result>
        IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email);
        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var isCorrectPassword =
            await userManager.CheckPasswordAsync(user, request.Request.OldPassword);
        if (!isCorrectPassword)
        {
            return Results.BadRequest($"Wrong password.");
        }

        var result = await userManager.ChangePasswordAsync(user, request.Request.OldPassword,
            request.Request.NewPassword);
        if (!result.Succeeded)
        {
            return Results.InternalServerError(
                $"Cannot change password due to server error: {result.Errors.First().Description}.");
        }

        return Result.Success("Password has been successfully changed.");
    }
}