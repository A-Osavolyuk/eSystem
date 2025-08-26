using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Roles.Commands;

public sealed record UnassignRolesCommand(UnassignRolesRequest Request)
    : IRequest<Result>;

public sealed class RemoveUserRolesCommandHandler(
    IUserManager userManager,
    IRoleManager roleManager)
    : IRequestHandler<UnassignRolesCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(UnassignRolesCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        foreach (var roleName in request.Request.Roles)
        {
            var role = await roleManager.FindByNameAsync(roleName, cancellationToken);
            if (role is null) return Results.NotFound($"Cannot find role with name {roleName}.");

            var result = await roleManager.UnassignAsync(user, role, cancellationToken);
            if (!result.Succeeded) return result;
        }

        return Result.Success();
    }
}