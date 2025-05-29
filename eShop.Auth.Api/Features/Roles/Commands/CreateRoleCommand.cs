using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Roles.Commands;

internal sealed record CreateRoleCommand(CreateRoleRequest Request) : IRequest<Result>;

internal sealed class CreateRoleCommandHandler(
    IRoleManager roleManager) : IRequestHandler<CreateRoleCommand, Result>
{
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var isRoleExists = await roleManager.FindByNameAsync(request.Request.Name, cancellationToken);

        if (isRoleExists is not null)
        {
            return Result.Success("Role already exists.");
        }

        var entity = Mapper.Map(request.Request);
        await roleManager.CreateAsync(entity, cancellationToken);

        return Result.Success("Role was successfully created");
    }
}