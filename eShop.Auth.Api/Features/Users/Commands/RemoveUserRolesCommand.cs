using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record RemoveUserRolesCommand(RemoveUserRolesRequest Request)
    : IRequest<Result>;

internal sealed class RemoveUserRolesCommandHandler(
    IUserManager userManager)
    : IRequestHandler<RemoveUserRolesCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(RemoveUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        foreach (var role in request.Request.Roles)
        {
            var isInRole = await userManager.IsInRoleAsync(user, role, cancellationToken);

            if (!isInRole)
            {
                return Results.BadRequest($"User with ID {request.Request.UserId} is not in role {role}.");
            }

            var result = await userManager.RemoveFromRoleAsync(user, role, cancellationToken);

            if (!result.Succeeded)
            {
                return result;
            }
        }

        return Result.Success("Roles were successfully removed from user");
    }
}