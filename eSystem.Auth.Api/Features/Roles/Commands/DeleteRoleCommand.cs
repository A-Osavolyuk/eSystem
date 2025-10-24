using eSystem.Auth.Api.Interfaces;

namespace eSystem.Auth.Api.Features.Roles.Commands;

public sealed record DeleteRoleCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteRoleCommandHandler(
    IRoleManager roleManager) : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id, cancellationToken);
        if (role is null) return Results.NotFound($"Cannot find role with ID {request.Id}.");
        
        var result = await roleManager.DeleteAsync(role, cancellationToken);

        return result;
    }
}