using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public sealed record GetUserRolesQuery(Guid Id) : IRequest<Result>;

public sealed class GetUserRolesQueryHandler(IUserManager userManager) : IRequestHandler<GetUserRolesQuery, Result>
{
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Id}.");

        var roles = user.Roles.Select(x => x.Role).ToList();
        if (!roles.Any()) return Results.NotFound($"Cannot find roles for user with ID {request.Id}.");

        var result = roles.Select(role => new RoleDto()
        {
            Id = role.Id,
            Name = role.Name,
            NormalizedName = role.NormalizedName
        }).ToList();
        
        return Result.Success(result);
    }
}