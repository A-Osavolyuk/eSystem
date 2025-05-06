using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Roles.Commands;

internal sealed record AssignRoleCommand(AssignRoleRequest Request) : IRequest<Result>;

internal sealed class AssignRoleCommandHandler(
    ILogger<AssignRoleCommandHandler> logger,
    UserManager<UserEntity> userManager,
    IRoleManager roleManager) : IRequestHandler<AssignRoleCommand, Result>
{
    private readonly ILogger<AssignRoleCommandHandler> logger = logger;
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByNameAsync(request.Request.RoleName, cancellationToken);
        var user = await userManager.FindByIdAsync(request.Request.UserId.ToString());

        if (role is null)
        {
            return Results.NotFound($"Cannot find role with name {request.Request.RoleName}");
        }

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var result = await userManager.AddToRoleAsync(user, role.Name!);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(
                $"Cannot assign role due to server error: {result.Errors.First().Description}");
        }

        return Result.Success("Role was successfully assigned");
    }
}