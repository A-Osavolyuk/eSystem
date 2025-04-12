using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;
using Error = eShop.Domain.Common.API.Error;

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
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var isCorrectPassword =
            await appManager.UserManager.CheckPasswordAsync(user, request.Request.OldPassword);
        if (!isCorrectPassword)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.BadRequest,
                Message = "Bad request",
                Details = $"Wrong password."
            });
        }

        var result = await appManager.UserManager.ChangePasswordAsync(user, request.Request.OldPassword,
            request.Request.NewPassword);
        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Internal server error",
                Details = $"Cannot change password due to server error: {result.Errors.First().Description}."
            });
        }

        return Result.Success("Password has been successfully changed.");
    }
}