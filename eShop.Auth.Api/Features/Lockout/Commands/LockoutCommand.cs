using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Lockout.Commands;

public record LockoutCommand(LockoutRequest Request) : IRequest<Result>;

public class LockoutCommandHandler(
    IUserManager userManager,
    ILockoutManager lockoutManager) : IRequestHandler<LockoutCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;

    public async Task<Result> Handle(LockoutCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        //TODO: Refactor lockout flow

        return Result.Success();
    }
}