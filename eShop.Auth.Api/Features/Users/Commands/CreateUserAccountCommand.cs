using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record CreateUserAccountCommand(CreateUserAccountRequest Request)
    : IRequest<Result>;

internal sealed class CreateUserAccountCommandHandler(
    IPermissionManager permissionManager,
    IProfileManager profileManager,
    ISecurityManager securityManager,
    IUserManager userManager,
    IRoleManager roleManager,
    IConfiguration configuration) : IRequestHandler<CreateUserAccountCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly ISecurityManager securityManager = securityManager;
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;
    private readonly string defaultRole = configuration["Configuration:General:DefaultValues:DefaultRole"]!;
    private readonly List<string> defaultPermissions =
        configuration.GetValue<List<string>>("Configuration:General:DefaultValues:DefaultPermissions")!;

    public async Task<Result> Handle(CreateUserAccountCommand request,
        CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();
        var user = new UserEntity()
        {
            Id = userId,
            Email = request.Request.Email,
            UserName = request.Request.UserName,
            PhoneNumber = request.Request.PhoneNumber,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            LockoutEnabled = false,
        };

        var accountResult = await userManager.CreateAsync(user, cancellationToken);

        if (!accountResult.Succeeded)
        {
            return accountResult;
        }

        var password = securityManager.GenerateRandomPassword(18);
        var passwordResult = await userManager.AddPasswordAsync(user, password, cancellationToken);

        if (!passwordResult.Succeeded)
        {
            return passwordResult;
        }

        await profileManager.SetAsync(user, new PersonalDataEntity()
        {
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            DateOfBirth = request.Request.DateOfBirth,
            Gender = request.Request.Gender
        }, cancellationToken);

        if (request.Request.Roles.Any())
        {
            foreach (var role in request.Request.Roles)
            {
                var roleExists = await roleManager.ExistsAsync(role, cancellationToken);

                if (!roleExists)
                {
                    return Results.InternalServerError($"Role {role} does not exist.");
                }

                var roleResult = await userManager.AddToRoleAsync(user, role, cancellationToken);

                if (!roleResult.Succeeded)
                {
                    return roleResult;
                }
            }
        }
        else
        {
            var roleResult = await userManager.AddToRoleAsync(user, defaultRole, cancellationToken);

            if (!roleResult.Succeeded)
            {
                return roleResult;
            }
        }

        if (request.Request.Permissions.Any())
        {
            foreach (var permission in request.Request.Permissions)
            {
                var entity = await permissionManager.FindByNameAsync(permission, cancellationToken);

                if (entity is null)
                {
                    return Results.NotFound($"Cannot find permission {permission}.");
                }

                var permissionResult = await permissionManager.IssueAsync(user, entity, cancellationToken);

                if (!permissionResult.Succeeded)
                {
                    return permissionResult;
                }
            }
        }
        else
        {
            foreach (var permission in defaultPermissions)
            {
                var entity = await permissionManager.FindByNameAsync(permission, cancellationToken);

                if (entity is null)
                {
                    return Results.NotFound($"Cannot find permission {permission}.");
                }
                
                var permissionResult = await permissionManager.IssueAsync(user, entity, cancellationToken);

                if (!permissionResult.Succeeded)
                {
                    return permissionResult;
                }
            }
        }

        return Result.Success($"User account was successfully created with temporary password: {password}");
    }
}