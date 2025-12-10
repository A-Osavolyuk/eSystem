using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authorization.Permissions;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public sealed record GetUserRolesQuery(Guid Id) : IRequest<Result>;

public sealed class GetUserRolesQueryHandler(
    IUserManager userManager,
    IRoleManager roleManager) : IRequestHandler<GetUserRolesQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IRoleManager _roleManager = roleManager;

    public async Task<Result> Handle(GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var roles = await _roleManager.GetAllAsync(user, cancellationToken);
        var result = roles.Select(x => x.Role).Select(role => new RoleDto()
        {
            Id = role.Id,
            Name = role.Name,
            NormalizedName = role.NormalizedName
        }).ToList();
        
        return Results.Ok(result);
    }
}