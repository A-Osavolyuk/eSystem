using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangePasswordCommand(ChangePasswordRequest Request)
    : IRequest<Result>;

internal sealed class ChangePasswordCommandHandler(AppManager appManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly AppManager appManager = appManager;

    async Task<Result>
        IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);
        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var isCorrectPassword =
            await appManager.UserManager.CheckPasswordAsync(user, request.Request.OldPassword);
        if (!isCorrectPassword)
        {
            return Results.BadRequest($"Wrong password.");
        }

        var result = await appManager.UserManager.ChangePasswordAsync(user, request.Request.OldPassword,
            request.Request.NewPassword);
        if (!result.Succeeded)
        {
            return Results.InternalServerError(
                $"Cannot change password due to server error: {result.Errors.First().Description}.");
        }

        return Result.Success("Password has been successfully changed.");
    }
}