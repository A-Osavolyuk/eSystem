using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Features.Roles.Commands;

public sealed record AssignRoleCommand(AssignRoleRequest Request) : IRequest<Result>;

public sealed class AssignRoleCommandHandler(
    IUserManager userManager,
    IRoleManager roleManager) : IRequestHandler<AssignRoleCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var role = await roleManager.FindByNameAsync(request.Request.RoleName, cancellationToken);
        if (role is null) return Results.NotFound($"Cannot find role with name {request.Request.RoleName}");

        var result = await roleManager.AssignAsync(user, role, cancellationToken);
        return result;
    }
}