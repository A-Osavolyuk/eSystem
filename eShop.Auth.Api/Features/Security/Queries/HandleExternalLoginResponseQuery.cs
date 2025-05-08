using eShop.Domain.Messages.Email;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record HandleExternalLoginResponseQuery(
    ExternalLoginInfo ExternalLoginInfo,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

internal sealed class HandleExternalLoginResponseQueryHandler(
    IPermissionManager permissionManager,
    ISecurityManager securityManager,
    ITokenManager tokenManager,
    IUserManager userManager,
    IConfiguration configuration,
    IMessageService messageService) : IRequestHandler<HandleExternalLoginResponseQuery, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly ISecurityManager securityManager = securityManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;
    private readonly string defaultRole = configuration["Configuration:General:DefaultValues:DefaultRole"]!;
    private readonly string defaultPermission = configuration["Configuration:General:DefaultValues:DefaultPermission"]!;

    public async Task<Result> Handle(HandleExternalLoginResponseQuery request,
        CancellationToken cancellationToken)
    {
        var email = request.ExternalLoginInfo.Principal.Claims
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

            var assignDefaultRoleResult = await userManager.AddToRoleAsync(user, defaultRole, cancellationToken);

            if (!assignDefaultRoleResult.Succeeded)
            {
                return assignDefaultRoleResult;
            }

            var issuingPermissionsResult =
                await permissionManager.IssueAsync(user, [defaultPermission], cancellationToken);

            if (!issuingPermissionsResult.Succeeded)
            {
                return Results.InternalServerError($"Cannot assign permissions for user with email {user.Email} " +
                                                   $"due to server error: {issuingPermissionsResult.Errors.First().Description}");
            }

            await messageService.SendMessageAsync("external-provider-registration", new ExternalRegistrationMessage()
            {
                To = email,
                Subject = $"Account registered with {request.ExternalLoginInfo!.ProviderDisplayName}",
                TempPassword = tempPassword,
                UserName = email,
                ProviderName = request.ExternalLoginInfo!.ProviderDisplayName!
            }, cancellationToken);
            
            var token = await tokenManager.GenerateAsync(user, cancellationToken);
            var link = UrlGenerator.ActionLink("/account/confirm-external-login", frontendUri,
                new { Token = token, ReturnUri = request.ReturnUri });
            return Result.Success(link);
        }
    }
}