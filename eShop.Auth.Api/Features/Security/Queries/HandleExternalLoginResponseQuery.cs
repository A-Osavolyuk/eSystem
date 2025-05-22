using System.Security.Claims;
using eShop.Domain.Messages.Email;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record HandleExternalLoginResponseQuery(
    ClaimsPrincipal Principal,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

internal sealed class HandleExternalLoginResponseQueryHandler(
    IPermissionManager permissionManager,
    ISecurityManager securityManager,
    ITokenManager tokenManager,
    IUserManager userManager,
    IConfiguration configuration,
    IMessageService messageService,
    IRoleManager roleManager) : IRequestHandler<HandleExternalLoginResponseQuery, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly ISecurityManager securityManager = securityManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

    public async Task<Result> Handle(HandleExternalLoginResponseQuery request,
        CancellationToken cancellationToken)
    {
        var email = request.Principal.Claims
            .FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

        if (email is null)
        {
            return Results.BadRequest("No email address specified in credentials.");
        }

        var user = await userManager.FindByEmailAsync(email, cancellationToken);

        if (user is not null)
        {
            var securityToken = await tokenManager.FindAsync(user, cancellationToken);

            if (securityToken is not null)
            {
                var token = await tokenManager.GenerateAsync(user, cancellationToken);

                var link = UrlGenerator.ActionLink("/account/confirm-external-login", frontendUri,
                    new { token.AccessToken, token.RefreshToken, request.ReturnUri });
                return Result.Success(link);
            }
            else
            {
                var tokens = await tokenManager.GenerateAsync(user, cancellationToken);
                var link = UrlGenerator.ActionLink("/account/confirm-external-login", frontendUri,
                    new { tokens!.AccessToken, tokens.RefreshToken, request.ReturnUri });
                return Result.Success(link);
            }
        }
        
        {
            user = new UserEntity()
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var tempPassword = securityManager.GenerateRandomPassword(18);
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
            
            var assignRoleResult = await roleManager.AssignRoleAsync(user, role, cancellationToken);

            if (!assignRoleResult.Succeeded)
            {
                return assignRoleResult;
            }

            var permissions = role.Permissions.Select(x => x.Permission).ToList();
            
            var grantPermissionsResult = await permissionManager.GrantAsync(user, permissions, cancellationToken);

            if (!grantPermissionsResult.Succeeded)
            {
                return grantPermissionsResult;
            }

            var provider = request.Principal.Identity!.AuthenticationType!;
            
            await messageService.SendMessageAsync("email:external-provider-registration", new ExternalRegistrationMessage()
            {
                To = email,
                Subject = $"Account registered with {provider}",
                TempPassword = tempPassword,
                UserName = email,
                ProviderName = provider!
            }, cancellationToken);
            
            var token = await tokenManager.GenerateAsync(user, cancellationToken);
            var link = UrlGenerator.ActionLink("/account/confirm-external-login", frontendUri,
                new { Token = token, ReturnUri = request.ReturnUri });
            return Result.Success(link);
        }
    }
}