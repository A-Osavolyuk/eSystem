using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record RemoveUserRoleCommand(RemoveUserRoleRequest Request) : IRequest<Result>;

internal sealed class RemoveUserRoleCommandHandler(
    AppManager appManager) : IRequestHandler<RemoveUserRoleCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(RemoveUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var isInRole = await appManager.UserManager.IsInRoleAsync(user, request.Request.Role);

        if (!isInRole)
        {
            return Results.NotFound($"User with ID {user.Id} not in role {request.Request.Role}");
        }

        var result = await appManager.UserManager.RemoveFromRoleAsync(user, request.Request.Role);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(
                $"Cannot remove user with ID {user.Id} from role {request.Request.Role}.");
        }

        return Result.Success("User was successfully removed from role.");
    }
}