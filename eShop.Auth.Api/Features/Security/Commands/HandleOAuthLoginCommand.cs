using System.Security.Claims;
using eShop.Auth.Api.Messages.Email;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record HandleOAuthLoginCommand(
    ClaimsPrincipal Principal,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

public sealed class HandleOAuthLoginCommandHandler(
    IPermissionManager permissionManager,
    ITokenManager tokenManager,
    IUserManager userManager,
    IMessageService messageService,
    IRoleManager roleManager,
    ILockoutManager lockoutManager) : IRequestHandler<HandleOAuthLoginCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;

    public async Task<Result> Handle(HandleOAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null)
        {
            return Results.BadRequest("No email address specified in credentials.");
        }

        var user = await userManager.FindByEmailAsync(email, cancellationToken);

        if (user is not null)
        {
            var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

            if (lockoutState.Enabled)
            {
                return Results.BadRequest($"This user account is locked out with reason: {lockoutState.Reason}.");
            }

            var link = await GenerateLinkAsync(user, request.ReturnUri!, cancellationToken);

            return Result.Success(link);
        }

        {
            user = new UserEntity()
            {
                Id = Guid.CreateVersion7(),
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var tempPassword = PasswordGenerator.Generate(18);
            var result = await userManager.CreateAsync(user, tempPassword, cancellationToken);

            if (!result.Succeeded)
            {
                return result;
            }

            var role = await roleManager.FindByNameAsync("User", cancellationToken);

            if (role is null)
            {
                return Results.NotFound("Cannot find role with name User");
            }

            var assignRoleResult = await roleManager.AssignAsync(user, role, cancellationToken);

            if (!assignRoleResult.Succeeded)
            {
                return assignRoleResult;
            }

            var permissions = role.Permissions.Select(x => x.Permission).ToList();

            foreach (var permission in permissions)
            {
                var grantResult = await permissionManager.GrantAsync(user, permission, cancellationToken);

                if (!grantResult.Succeeded)
                {
                    return grantResult;
                }
            }

            var provider = request.Principal.Identity!.AuthenticationType!;

            var message = new OAuthLoginMessage()
            {
                Credentials = new Dictionary<string, string>()
                {
                    { "To", email },
                    { "Subject", $"Account registered with {provider}" },
                },
                Payload = new()
                {
                    { "UserName", user.UserName },
                    { "ProviderName", provider },
                    { "TempPassword", tempPassword }
                }
            };

            await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

            var link = await GenerateLinkAsync(user, request.ReturnUri!, cancellationToken);

            return Result.Success(link);
        }
    }

    private async Task<string> GenerateLinkAsync(UserEntity user, string returnUri,
        CancellationToken cancellationToken = default)
    {
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);
        var link = UrlGenerator.ActionLink(returnUri, new { accessToken, refreshToken });

        return link;
    }
}