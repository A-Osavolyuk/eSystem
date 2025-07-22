using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Users.Commands;

public sealed record CreateUserAccountCommand(CreateUserAccountRequest Request)
    : IRequest<Result>;

public sealed class CreateUserAccountCommandHandler(
    IPermissionManager permissionManager,
    IPersonalDataManager personalDataManager,
    IUserManager userManager,
    IRoleManager roleManager) : IRequestHandler<CreateUserAccountCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;

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
        };

        var password = PasswordGenerator.Generate(18);
        var accountResult = await userManager.CreateAsync(user, password, cancellationToken);

        if (!accountResult.Succeeded)
        {
            return accountResult;
        }

        await personalDataManager.CreateAsync(new PersonalDataEntity()
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

                var roleResult = await roleManager.AssignAsync(user, role, cancellationToken);

                if (!roleResult.Succeeded)
                {
                    return roleResult;
                }
            }
        }
        else
        {
            var role = await roleManager.FindByNameAsync("User", cancellationToken);

            if (role is null)
            {
                return Results.NotFound("Cannot find role User");
            }
            
            var roleResult = await roleManager.AssignAsync(user, "User", cancellationToken);

            if (!roleResult.Succeeded)
            {
                return roleResult;
            }

            var permissions = role.Permissions.Select(x => x.Permission).ToList();
            var permissionResult = await permissionManager.GrantAsync(user, permissions, cancellationToken);

            if (!permissionResult.Succeeded)
            {
                return permissionResult;
            }
        }

        if (request.Request.Permissions.Count > 0)
        {
            foreach (var permission in request.Request.Permissions)
            {
                var entity = await permissionManager.FindByNameAsync(permission, cancellationToken);

                if (entity is null)
                {
                    return Results.NotFound($"Cannot find permission {permission}.");
                }

                var permissionResult = await permissionManager.GrantAsync(user, entity, cancellationToken);

                if (!permissionResult.Succeeded)
                {
                    return permissionResult;
                }
            }
        }

        return Result.Success($"User account was successfully created with temporary password: {password}");
    }
}