using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record RemoveUserRolesCommand(RemoveUserRolesRequest Request)
    : IRequest<Result>;

internal sealed class RemoveUserRolesCommandHandler(
    UserManager<UserEntity> userManager)
    : IRequestHandler<RemoveUserRolesCommand, Result>
{
    private readonly UserManager<UserEntity> userManager = userManager;

    public async Task<Result> Handle(RemoveUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        foreach (var role in request.Request.Roles)
        {
            var isInRole = await userManager.IsInRoleAsync(user, role);

            if (!isInRole)
            {
                return Results.BadRequest($"User with ID {request.Request.UserId} is not in role {role}.");
            }

            var result = await userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return Results.InternalServerError($"Cannot remove role from user with ID {request.Request.UserId} " +
                                                   $"due to server error: {result.Errors.First().Description}.");
            }
        }

        return Result.Success("Roles were successfully removed from user");
    }
}