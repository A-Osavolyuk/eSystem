using eShop.Domain.Requests.API.Admin;
using eShop.Domain.Responses.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record LockoutUserCommand(LockoutUserRequest Request) : IRequest<Result>;

internal sealed class LockoutUserCommandHandler(
    UserManager<UserEntity> userManager) : IRequestHandler<LockoutUserCommand, Result>
{
    public async Task<Result> Handle(LockoutUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        if (request.Request.Permanent)
        {
            var lockoutEndDate = DateTime.UtcNow.AddYears(100);
            await userManager.SetLockoutEnabledAsync(user, true);
            await userManager.SetLockoutEndDateAsync(user, lockoutEndDate);

            return Result.Success(new LockoutUserResponse()
            {
                LockoutEnabled = true,
                LockoutEnd = lockoutEndDate,
                Message = "User was successfully permanently banned.",
                Succeeded = true
            });
        }

        await userManager.SetLockoutEnabledAsync(user, true);
        await userManager.SetLockoutEndDateAsync(user, request.Request.LockoutEnd);

        return Result.Success(new LockoutUserResponse()
        {
            LockoutEnabled = true,
            LockoutEnd = request.Request.LockoutEnd,
            Message = $"User was successfully banned until {request.Request.LockoutEnd}.",
            Succeeded = true
        });
    }
}