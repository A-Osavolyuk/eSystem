using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.Roles.Commands;

public record UpdateRoleCommand(UpdateRoleRequest Request) :  IRequest<Result>;

public class UpdateRoleCommandHandler(IRoleManager roleManager) : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Request.Id, cancellationToken);
        if (role is null) return Results.NotFound($"Cannot find role with ID {request.Request.Id}");
        
        role.Name = request.Request.Name;
        role.NormalizedName = request.Request.Name.ToUpper();
        role.UpdateDate = DateTimeOffset.UtcNow;
        
        var result = await roleManager.UpdateAsync(role, cancellationToken);
        
        return result;
    }
}