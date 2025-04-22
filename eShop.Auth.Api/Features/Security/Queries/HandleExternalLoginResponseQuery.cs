using eShop.Domain.Common.API;
using eShop.Domain.Messages.Email;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record HandleExternalLoginResponseQuery(
    ExternalLoginInfo ExternalLoginInfo,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

internal sealed class HandleExternalLoginResponseQueryHandler(
    AppManager appManager,
    ITokenHandler tokenHandler,
    IConfiguration configuration,
    IMessageService messageService) : IRequestHandler<HandleExternalLoginResponseQuery, Result>
{
    private readonly IConfiguration configuration = configuration;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;
    private readonly string defaultRole = configuration["Configuration:General:DefaultValues:DefaultRole"]!;
    private readonly string defaultPermission = configuration["Configuration:General:DefaultValues:DefaultPermission"]!;

    public async Task<Result> Handle(HandleExternalLoginResponseQuery request,
        CancellationToken cancellationToken)
    {
        var email = request.ExternalLoginInfo.Principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null)
        {
            return Results.BadRequest("No email address specified in credentials.");
        }

        var user = await appManager.UserManager.FindByEmailAsync(email);

        if (user is not null)
        {
            var securityToken = await appManager.SecurityManager.FindTokenAsync(user);

            if (securityToken is not null)
            {
                var token = await tokenHandler.RefreshTokenAsync(user, securityToken.Token);

                var link = UrlGenerator.ActionLink("/account/confirm-external-login", frontendUri,
                    new { token, request.ReturnUri });
                return Result.Success(link);
            }
            else
            {
                var tokens = await tokenHandler.GenerateTokenAsync(user);
                var link = UrlGenerator.ActionLink("/account/confirm-external-login", frontendUri,
                    new { tokens!.AccessToken, tokens.RefreshToken, request.ReturnUri });
                return Result.Success(link);
            }
        }

        {
            user = new AppUser()
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var tempPassword = appManager.SecurityManager.GenerateRandomPassword(18);
            var result = await appManager.UserManager.CreateAsync(user, tempPassword);

            if (!result.Succeeded)
            {
                return Results.InternalServerError(
                    $"Cannot create user account due to server error: {result.Errors.First().Description}");
            }

            var assignDefaultRoleResult = await appManager.UserManager.AddToRoleAsync(user, defaultRole);

            if (!assignDefaultRoleResult.Succeeded)
            {
                return Results.InternalServerError($"Cannot assign role {defaultRole} to user with email {user.Email}" +
                                                   $"due to server error: {assignDefaultRoleResult.Errors.First().Description}");
            }

            var issuingPermissionsResult =
                await appManager.PermissionManager.IssueAsync(user, [defaultPermission], cancellationToken);

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
            
            var token = await tokenHandler.GenerateTokenAsync(user);
            var link = UrlGenerator.ActionLink("/account/confirm-external-login", frontendUri,
                new { Token = token, ReturnUri = request.ReturnUri });
            return Result.Success(link);
        }
    }
}