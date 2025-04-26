using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Roles.Commands;

internal sealed record CreateRoleCommand(CreateRoleRequest Request) : IRequest<Result>;

internal sealed class CreateRoleCommandHandler(
    RoleManager<RoleEntity> roleManager) : IRequestHandler<CreateRoleCommand, Result>
{
    private readonly RoleManager<RoleEntity> roleManager = roleManager;

    public async Task<Result> Handle(CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var isRoleExists = await roleManager.FindByNameAsync(request.Request.Name);

        if (isRoleExists is not null)
        {
            return Result.Success("Role already exists.");
        }

        var result = await roleManager.CreateAsync(new RoleEntity() { Name = request.Request.Name });

        if (!result.Succeeded)
        {
            return Results.InternalServerError(
                $"Cannot create role due to server error: {result.Errors.First().Description}");
        }

        return Result.Success("Role was successfully created");
    }
}