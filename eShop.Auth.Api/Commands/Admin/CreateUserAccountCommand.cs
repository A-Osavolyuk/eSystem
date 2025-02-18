namespace eShop.Auth.Api.Commands.Admin;

internal sealed record CreateUserAccountCommand(CreateUserAccountRequest Request)
    : IRequest<Result<CreateUserAccountResponse>>;

internal sealed class CreateUserAccountCommandHandler(
    AppManager appManager,
    IConfiguration configuration) : IRequestHandler<CreateUserAccountCommand, Result<CreateUserAccountResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly string defaultRole = configuration["Configuration:General:DefaultValues:DefaultRole"]!;

    private readonly List<string> defaultPermissions =
        configuration.GetValue<List<string>>("Configuration:General:DefaultValues:DefaultPermissions")!;

    public async Task<Result<CreateUserAccountResponse>> Handle(CreateUserAccountCommand request,
        CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();
        var user = new AppUser()
        {
            Id = userId.ToString(),
            Email = request.Request.Email,
            UserName = request.Request.UserName,
            PhoneNumber = request.Request.PhoneNumber,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            LockoutEnabled = false,
        };

        var accountResult = await appManager.UserManager.CreateAsync(user);

        if (!accountResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot create account due to server error: {accountResult.Errors.First().Description}."));
        }

        var password = appManager.SecurityManager.GenerateRandomPassword(18);
        var passwordResult = await appManager.UserManager.AddPasswordAsync(user, password);

        if (!passwordResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot add password to user account due ti server error: {passwordResult.Errors.First().Description}."));
        }

        await appManager.ProfileManager.SetPersonalDataAsync(user, new PersonalDataEntity()
        {
            UserId = userId.ToString(),
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            DateOfBirth = request.Request.DateOfBirth,
            Gender = request.Request.Gender
        });

        if (request.Request.Roles.Any())
        {
            foreach (var role in request.Request.Roles)
            {
                var roleExists = await appManager.RoleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    return new(new NotFoundException($"Cannot find role {role}."));
                }

                var roleResult = await appManager.UserManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    return new(new FailedOperationException($"Cannot add user with ID {user.Id} to role {role} " +
                                                            $"due to server error: {roleResult.Errors.First().Description}."));
                }
            }
        }
        else
        {
            var roleResult = await appManager.UserManager.AddToRoleAsync(user, defaultRole);

            if (!roleResult.Succeeded)
            {
                return new(new FailedOperationException(
                    $"Cannot add user with ID {user.Id} to role {defaultRole} " +
                    $"due to server error: {roleResult.Errors.First().Description}."));
            }
        }

        if (request.Request.Permissions.Any())
        {
            foreach (var permission in request.Request.Permissions)
            {
                var permissionExists = await appManager.PermissionManager.ExistsAsync(permission);

                if (!permissionExists)
                {
                    return new(new NotFoundException($"Cannot find permission {permission}."));
                }

                var permissionResult =
                    await appManager.PermissionManager.IssuePermissionAsync(user, permission);

                if (!permissionResult.Succeeded)
                {
                    return new(new FailedOperationException(
                        $"Cannot issue permission {permission} to user with ID {user.Id} due to " +
                        $"server error: {permissionResult.Errors.First().Description}."));
                }
            }
        }
        else
        {
            foreach (var permission in defaultPermissions)
            {
                var permissionResult =
                    await appManager.PermissionManager.IssuePermissionAsync(user, permission);

                if (!permissionResult.Succeeded)
                {
                    return new(new FailedOperationException(
                        $"Cannot issue permission {permission} to user with ID {user.Id} due to " +
                        $"server error: {permissionResult.Errors.First().Description}."));
                }
            }
        }

        return new(new CreateUserAccountResponse()
        {
            Succeeded = true,
            Message = $"User account was successfully created with temporary password: {password}"
        });
    }
}