using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Roles.Commands;

internal sealed record DeleteRoleCommand(DeleteRoleRequest Request) : IRequest<Result>;

internal sealed class DeleteRoleCommandHandler(
    RoleManager<RoleEntity> roleManager) : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly RoleManager<RoleEntity> roleManager = roleManager;

    public async Task<Result> Handle(DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Request.Id.ToString());

        if (role is null || role.Id != request.Request.Id)
        {
            return Results.NotFound($"Cannot find role with ID {request.Request.Id}.");
        }

        if (role.Name != request.Request.Name)
        {
            return Results.BadRequest("Role name is different.");
        }

        var result = await roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return Results.InternalServerError($"Cannot delete role due to server error: {result.Errors.First()}.");
        }

        return Result.Success("Role was successfully deleted.");
    }
}