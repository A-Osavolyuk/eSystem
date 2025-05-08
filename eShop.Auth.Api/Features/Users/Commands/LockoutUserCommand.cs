using eShop.Domain.Requests.API.Admin;
using eShop.Domain.Responses.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record LockoutUserCommand(LockoutUserRequest Request) : IRequest<Result>;

internal sealed class LockoutUserCommandHandler(
    IUserManager userManager,
    ILockoutManager lockoutManager) : IRequestHandler<LockoutUserCommand, Result>
{
    private readonly ILockoutManager lockoutManager = lockoutManager;

    public async Task<Result> Handle(LockoutUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        if (request.Request.Permanent)
        {
            var lockoutEndDate = DateTime.UtcNow.AddYears(100);
            await lockoutManager.EnableAsync(user, lockoutEndDate, cancellationToken);

            return Result.Success(new LockoutUserResponse()
            {
                LockoutEnabled = true,
                LockoutEnd = lockoutEndDate,
                Message = "User was successfully permanently banned.",
                Succeeded = true
            });
        }

        await lockoutManager.EnableAsync(user, request.Request.LockoutEnd, cancellationToken);
        await lockoutManager.SetEndDateAsync(user, request.Request.LockoutEnd, cancellationToken);

        return Result.Success(new LockoutUserResponse()
        {
            LockoutEnabled = true,
            LockoutEnd = request.Request.LockoutEnd,
            Message = $"User was successfully banned until {request.Request.LockoutEnd}.",
            Succeeded = true
        });
    }
}