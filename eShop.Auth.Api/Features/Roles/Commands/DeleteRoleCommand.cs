namespace eShop.Auth.Api.Features.Roles.Commands;

internal sealed record DeleteRoleCommand(Guid Id) : IRequest<Result>;

internal sealed class DeleteRoleCommandHandler(
    IRoleManager roleManager) : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id, cancellationToken);

        if (role is null)
        {
            return Results.NotFound($"Cannot find role with ID {request.Id}.");
        }
        
        await roleManager.DeleteAsync(role, cancellationToken);

        return Result.Success();
    }
}