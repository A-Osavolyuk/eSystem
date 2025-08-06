using System.Security.Claims;
using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Types;
using OtpNet;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record HandleOAuthLoginCommand(
    AuthenticationResult AuthenticationResult,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

public sealed class HandleOAuthLoginCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IMessageService messageService,
    IRoleManager roleManager,
    ILockoutManager lockoutManager,
    IOAuthSessionManager sessionManager,
    IOAuthProviderManager providerManager) : IRequestHandler<HandleOAuthLoginCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly IOAuthProviderManager providerManager = providerManager;

    public async Task<Result> Handle(HandleOAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        var authenticationResult = request.AuthenticationResult;
        var items = authenticationResult.Properties.Items;
        var fallbackUri = items["fallbackUri"]!;
        var providerName = request.AuthenticationResult.Principal.Identity!.AuthenticationType!;

        if (!string.IsNullOrEmpty(request.RemoteError))
        {
            var error = Uri.EscapeDataString(request.RemoteError);
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.RemoteError),
                Message = error,
                Provider = providerName
            });

            return Results.Redirect(url);
        }

        var provider = await providerManager.FindByNameAsync(providerName, cancellationToken);

        if (provider is null)
        {
            var error = Uri.EscapeDataString($"Cannot find provider {providerName}");
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = providerName
            });

            return Results.Redirect(url);
        }

        var email = request.AuthenticationResult.Principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null)
        {
            var error = Uri.EscapeDataString("Email was not provided to credentials");
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InvalidCredentials),
                Message = error,
                Provider = providerName
            });

            return Results.Redirect(url);
        }

        var sessionId = Guid.Parse(items["sessionId"]!);
        var token = items["token"]!;

        var session = await sessionManager.FindAsync(sessionId, token, cancellationToken);

        if (session is null)
        {
            var error = Uri.EscapeDataString("Invalid OAuth session");
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InvalidCredentials),
                Message = error,
                Provider = providerName
            });

            return Results.Redirect(url);
        }

        var user = await userManager.FindByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            user = new UserEntity()
            {
                Id = Guid.CreateVersion7(),
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, cancellationToken);

            if (!createResult.Succeeded)
            {
                var error = Uri.EscapeDataString(createResult.GetError().Message);
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.InternalError),
                    Message = error,
                    Provider = providerName
                });

                return Results.Redirect(url);
            }

            var role = await roleManager.FindByNameAsync("User", cancellationToken);

            if (role is null)
            {
                var error = Uri.EscapeDataString("Cannot find role with name User");
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.InternalError),
                    Message = error,
                    Provider = providerName
                });

                return Results.Redirect(url);
            }

            var assignRoleResult = await roleManager.AssignAsync(user, role, cancellationToken);

            if (!assignRoleResult.Succeeded)
            {
                var error = Uri.EscapeDataString(assignRoleResult.GetError().Message);
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.InternalError),
                    Message = error,
                    Provider = providerName
                });

                return Results.Redirect(url);
            }

            if (role.Permissions.Count > 0)
            {
                var permissions = role.Permissions.Select(x => x.Permission).ToList();

                foreach (var permission in permissions)
                {
                    var result = await permissionManager.GrantAsync(user, permission, cancellationToken);

                    if (result.Succeeded) continue;

                    var error = Uri.EscapeDataString(result.GetError().Message);
                    var url = UrlGenerator.Url(fallbackUri, new
                    {
                        ErrorCode = nameof(OAuthErrorType.InternalError),
                        Message = error,
                        Provider = providerName
                    });

                    return Results.Redirect(url);
                }
            }

            var message = new OAuthSignUpMessage()
            {
                Credentials = new Dictionary<string, string>()
                {
                    { "To", email },
                    { "Subject", $"Account registered with {providerName}" },
                },
                Payload = new()
                {
                    { "UserName", user.UserName },
                    { "ProviderName", providerName }
                }
            };

            await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

            session.SignType = OAuthSignType.SignUp;
        }
        else
        {
            var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

            if (lockoutState.Enabled)
            {
                var error = Uri.EscapeDataString(
                    $"This user account is locked out with reason: {lockoutState.Reason}.");
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.InternalError),
                    Message = error,
                    Provider = providerName
                });

                return Results.Redirect(url);
            }

            session.SignType = OAuthSignType.SignIn;
        }

        session.UserId = user.Id;

        var sessionResult = await sessionManager.UpdateAsync(session, cancellationToken);

        if (sessionResult.Succeeded)
        {
            var error = Uri.EscapeDataString(sessionResult.GetError().Message);
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = providerName
            });

            return Results.Redirect(url);
        }

        var enableResult = await providerManager.EnableAsync(user, provider, cancellationToken);

        if (!enableResult.Succeeded)
        {
            var error = Uri.EscapeDataString(enableResult.GetError().Message);
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = providerName
            });

            return Results.Redirect(url);
        }

        var link = UrlGenerator.Url(request.ReturnUri!, new { sessionId = session.Id });

        return Result.Success(link);
    }
}