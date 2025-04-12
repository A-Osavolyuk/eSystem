using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record CreateUserAccountCommand(CreateUserAccountRequest Request)
    : IRequest<Result>;

internal sealed class CreateUserAccountCommandHandler(
    AppManager appManager,
    IConfiguration configuration) : IRequestHandler<CreateUserAccountCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly string defaultRole = configuration["Configuration:General:DefaultValues:DefaultRole"]!;

    private readonly List<string> defaultPermissions =
        configuration.GetValue<List<string>>("Configuration:General:DefaultValues:DefaultPermissions")!;

    public async Task<Result> Handle(CreateUserAccountCommand request,
        CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();
        var user = new AppUser()
        {
            Id = userId,
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
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot create account due to server error: {accountResult.Errors.First().Description}."
            });
        }

        var password = appManager.SecurityManager.GenerateRandomPassword(18);
        var passwordResult = await appManager.UserManager.AddPasswordAsync(user, password);

        if (!passwordResult.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details =
                    $"Cannot add password to user account due ti server error: {passwordResult.Errors.First().Description}."
            });
        }

        await appManager.ProfileManager.SetPersonalDataAsync(user, new PersonalDataEntity()
        {
            UserId = userId,
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
                    return Result.Failure(new Error()
                    {
                        Code = ErrorCode.NotFound,
                        Message = "Not found",
                        Details = $"Role {role} does not exist."
                    });
                }

                var roleResult = await appManager.UserManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    return Result.Failure(new Error()
                    {
                        Code = ErrorCode.InternalServerError,
                        Message = "Server error",
                        Details = $"Cannot add user with ID {user.Id} to role {role} " +
                                  $"due to server error: {roleResult.Errors.First().Description}."
                    });
                }
            }
        }
        else
        {
            var roleResult = await appManager.UserManager.AddToRoleAsync(user, defaultRole);

            if (!roleResult.Succeeded)
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Server error",
                    Details = $"Cannot add user with ID {user.Id} to role {defaultRole} " +
                              $"due to server error: {roleResult.Errors.First().Description}."
                });
            }
        }

        if (request.Request.Permissions.Any())
        {
            foreach (var permission in request.Request.Permissions)
            {
                var permissionExists = await appManager.PermissionManager.ExistsAsync(permission);

                if (!permissionExists)
                {
                    return Result.Failure(new Error()
                    {
                        Code = ErrorCode.NotFound,
                        Message = "Not found",
                        Details = $"Cannot find permission {permission}."
                    });
                }

                var permissionResult =
                    await appManager.PermissionManager.IssuePermissionAsync(user, permission);

                if (!permissionResult.Succeeded)
                {
                    return Result.Failure(new Error()
                    {
                        Code = ErrorCode.InternalServerError,
                        Message = "Server error",
                        Details = $"Cannot issue permission {permission} to user with ID {user.Id} due to " +
                                  $"server error: {permissionResult.Errors.First().Description}."
                    });
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
                    return Result.Failure(new Error()
                    {
                        Code = ErrorCode.InternalServerError,
                        Message = "Server error",
                        Details = $"Cannot issue permission {permission} to user with ID {user.Id} due to " +
                                  $"server error: {permissionResult.Errors.First().Description}."
                    });
                }
            }
        }

        return Result.Success($"User account was successfully created with temporary password: {password}");
    }
}