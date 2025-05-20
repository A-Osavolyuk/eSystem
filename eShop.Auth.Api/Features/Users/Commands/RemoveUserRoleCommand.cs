using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record RemoveUserRoleCommand(UnassignRoleRequest Request) : IRequest<Result>;

internal sealed class RemoveUserRoleCommandHandler(
    IUserManager userManager) : IRequestHandler<RemoveUserRoleCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(RemoveUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var isInRole = await userManager.IsInRoleAsync(user, request.Request.Role, cancellationToken);

        if (!isInRole)
        {
            return Results.NotFound($"User with ID {user.Id} not in role {request.Request.Role}");
        }

        var result = await userManager.RemoveFromRoleAsync(user, request.Request.Role, cancellationToken);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(
                $"Cannot remove user with ID {user.Id} from role {request.Request.Role}.");
        }

        return Result.Success("User was successfully removed from role.");
    }
}