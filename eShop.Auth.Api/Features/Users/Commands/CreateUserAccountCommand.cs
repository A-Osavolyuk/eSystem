using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record CreateUserAccountCommand(CreateUserAccountRequest Request)
    : IRequest<Result>;

internal sealed class CreateUserAccountCommandHandler(
    IPermissionManager permissionManager,
    IProfileManager profileManager,
    ISecurityManager securityManager,
    UserManager<UserEntity> userManager,
    IRoleManager roleManager,
    IConfiguration configuration) : IRequestHandler<CreateUserAccountCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly ISecurityManager securityManager = securityManager;
    private readonly UserManager<UserEntity> userManager = userManager;
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

        var accountResult = await userManager.CreateAsync(user);

        if (!accountResult.Succeeded)
        {
            return Results.NotFound(
                $"Cannot create account due to server error: {accountResult.Errors.First().Description}.");
        }

        var password = securityManager.GenerateRandomPassword(18);
        var passwordResult = await userManager.AddPasswordAsync(user, password);

        if (!passwordResult.Succeeded)
        {
            return Results.NotFound(
                $"Cannot add password to user account due ti server error: {passwordResult.Errors.First().Description}.");
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

                var roleResult = await userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    return Results.InternalServerError($"Cannot add user with ID {user.Id} to role {role} " +
                                                       $"due to server error: {roleResult.Errors.First().Description}.");
                }
            }
        }
        else
        {
            var roleResult = await userManager.AddToRoleAsync(user, defaultRole);

            if (!roleResult.Succeeded)
            {
                return Results.InternalServerError($"Cannot add user with ID {user.Id} to role {defaultRole} " +
                                                   $"due to server error: {roleResult.Errors.First().Description}.");
            }
        }

        if (request.Request.Permissions.Any())
        {
            foreach (var permission in request.Request.Permissions)
            {
                var permissionExists = await permissionManager.ExistsAsync(permission, cancellationToken);

                if (!permissionExists)
                {
                    return Results.NotFound($"Cannot find permission {permission}.");
                }

                var permissionResult =
                    await permissionManager.IssueAsync(user, permission, cancellationToken);

                if (!permissionResult.Succeeded)
                {
                    return Results.InternalServerError(
                        $"Cannot issue permission {permission} to user with ID {user.Id} due to " +
                        $"server error: {permissionResult.Errors.First().Description}.");
                }
            }
        }
        else
        {
            foreach (var permission in defaultPermissions)
            {
                var permissionResult =
                    await permissionManager.IssueAsync(user, permission, cancellationToken);

                if (!permissionResult.Succeeded)
                {
                    return Results.InternalServerError(
                        $"Cannot issue permission {permission} to user with ID {user.Id} due to " +
                        $"server error: {permissionResult.Errors.First().Description}.");
                }
            }
        }

        return Result.Success($"User account was successfully created with temporary password: {password}");
    }
}