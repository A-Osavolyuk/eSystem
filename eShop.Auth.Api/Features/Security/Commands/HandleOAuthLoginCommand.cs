using System.Security.Claims;
using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Types;
using OtpNet;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record HandleOAuthLoginCommand(
    HttpContext Context,
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
    IOAuthProviderManager oAuthProviderManager,
    IProviderManager providerManager,
    ILoginSessionManager loginSessionManager) : IRequestHandler<HandleOAuthLoginCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly IOAuthProviderManager oAuthProviderManager = oAuthProviderManager;
    private readonly IProviderManager providerManager = providerManager;
    private readonly ILoginSessionManager loginSessionManager = loginSessionManager;

    public async Task<Result> Handle(HandleOAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        var authenticationResult = request.AuthenticationResult;
        var items = authenticationResult.Properties.Items;
        var providerName = request.AuthenticationResult.Principal.Identity!.AuthenticationType!;
        var fallbackUri = items["fallbackUri"]!;

        if (!string.IsNullOrEmpty(request.RemoteError))
        {
            return RedirectWithError(OAuthErrorType.RemoteError,
                providerName, request.RemoteError, fallbackUri);
        }

        var provider = await oAuthProviderManager.FindByNameAsync(providerName, cancellationToken);

        if (provider is null)
        {
            return RedirectWithError(OAuthErrorType.InternalError,
                providerName, $"Cannot find provider {providerName}", fallbackUri);
        }

        var email = request.AuthenticationResult.Principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null)
        {
            return RedirectWithError(OAuthErrorType.InvalidCredentials,
                provider.Name, "Email was not provided to credentials", fallbackUri);
        }

        var sessionId = Guid.Parse(items["sessionId"]!);
        var token = items["token"]!;

        var session = await sessionManager.FindAsync(sessionId, token, cancellationToken);

        if (session is null)
        {
            return RedirectWithError(OAuthErrorType.InvalidCredentials,
                provider.Name, "Invalid OAuth session", fallbackUri);
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

            var createResult = await CreateAccountAsync(user, provider, fallbackUri, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            var role = await roleManager.FindByNameAsync("User", cancellationToken);

            if (role is null)
            {
                return RedirectWithError(OAuthErrorType.InternalError,
                    provider.Name, "Cannot find role with name User", fallbackUri);
            }

            var assignResult = await AssignRoleAsync(user, role, provider, fallbackUri, cancellationToken);
            if (!assignResult.Succeeded) return assignResult;

            if (role.Permissions.Count > 0)
            {
                var grantResult = await GrantPermissionsAsync(user, role, provider, fallbackUri, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }

            var twoFactorResult = await AddTwoFactorProvidersAsync(user, fallbackUri, cancellationToken);
            if (!twoFactorResult.Succeeded) return twoFactorResult;

            await SendMessageAsync(user, provider, cancellationToken);

            session.SignType = OAuthSignType.SignUp;
            session.UserId = user.Id;

            var sessionResult = await UpdateSessionAsync(user, session, provider, fallbackUri, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;

            return await SignUpAsync(user, provider, session,
                request.ReturnUri!, fallbackUri, token, cancellationToken);
        }

        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled)
        {
            return await LockedOutAsync(user, lockoutState, provider, request.Context, fallbackUri, cancellationToken);
        }

        session.SignType = OAuthSignType.SignIn;
        session.UserId = user.Id;

        var updateResult = await UpdateSessionAsync(user, session, provider,
            request.Context, fallbackUri, cancellationToken);

        if (!updateResult.Succeeded) return updateResult;

        var providerAllowedResult = await IsProviderAllowedAsync(user, provider,
            request.Context, fallbackUri, cancellationToken);

        if (!providerAllowedResult.Succeeded) return providerAllowedResult;

        var checkProviderResult = await CheckUserProviderAsync(user, provider, request.Context,
            fallbackUri, cancellationToken);

        if (!checkProviderResult.Succeeded) return checkProviderResult;

        return await SignInAsync(user, provider, session, request.Context,
            request.ReturnUri!, fallbackUri, token, cancellationToken);
    }

    private async Task<Result> AddTwoFactorProvidersAsync(UserEntity user, string fallbackUri,
        CancellationToken cancellationToken = default)
    {
        var providers = await providerManager.GetAllAsync(cancellationToken);

        foreach (var provider in providers)
        {
            var providerResult = await providerManager.SubscribeAsync(user, provider, cancellationToken);
            if (!providerResult.Succeeded)
            {
                var error = Uri.EscapeDataString(providerResult.GetError().Message);
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = OAuthErrorType.InternalError,
                    Message = error,
                    Provider = provider
                });

                return Results.Redirect(url);
            }
        }

        return Result.Success();
    }

    private Result RedirectWithError(OAuthErrorType type, string provider, string message, string fallbackUri)
    {
        var error = Uri.EscapeDataString(message);
        var url = UrlGenerator.Url(fallbackUri, new
        {
            ErrorCode = type.ToString(),
            Message = error,
            Provider = provider
        });

        return Results.Redirect(url);
    }

    private async Task<Result> CreateAccountAsync(UserEntity user, OAuthProviderEntity provider,
        string fallbackUri, CancellationToken cancellationToken)
    {
        var createResult = await userManager.CreateAsync(user, cancellationToken);

        if (!createResult.Succeeded)
        {
            var error = Uri.EscapeDataString(createResult.GetError().Message);
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = provider.Name
            });

            return Results.Redirect(url);
        }

        return Result.Success();
    }

    private async Task<Result> AssignRoleAsync(UserEntity user, RoleEntity role, OAuthProviderEntity provider,
        string fallbackUri, CancellationToken cancellationToken)
    {
        var assignRoleResult = await roleManager.AssignAsync(user, role, cancellationToken);

        if (!assignRoleResult.Succeeded)
        {
            var error = Uri.EscapeDataString(assignRoleResult.GetError().Message);
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = provider.Name
            });

            return Results.Redirect(url);
        }

        return Result.Success();
    }

    private async Task<Result> GrantPermissionsAsync(UserEntity user, RoleEntity role, OAuthProviderEntity provider,
        string fallbackUri, CancellationToken cancellationToken)
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
                Provider = provider.Name
            });

            return Results.Redirect(url);
        }

        return Result.Success();
    }

    private async Task SendMessageAsync(UserEntity user, OAuthProviderEntity provider,
        CancellationToken cancellationToken)
    {
        var message = new OAuthSignUpMessage()
        {
            Credentials = new Dictionary<string, string>()
            {
                { "To", user.Email },
                { "Subject", $"Account registered with {provider.Name}" },
            },
            Payload = new()
            {
                { "UserName", user.UserName },
                { "ProviderName", provider.Name }
            }
        };

        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);
    }

    private async Task<Result> UpdateSessionAsync(UserEntity user, OAuthSessionEntity session,
        OAuthProviderEntity provider, string fallbackUri, CancellationToken cancellationToken)
    {
        var sessionResult = await sessionManager.UpdateAsync(session, cancellationToken);

        if (!sessionResult.Succeeded)
        {
            var error = Uri.EscapeDataString(sessionResult.GetError().Message);
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = provider.Name
            });

            return Results.Redirect(url);
        }

        return Result.Success();
    }

    private async Task<Result> SignUpAsync(UserEntity user, OAuthProviderEntity provider, OAuthSessionEntity session,
        string returnUri, string fallbackUri, string token, CancellationToken cancellationToken)
    {
        var enableResult = await oAuthProviderManager.EnableAsync(user, provider, cancellationToken);

        if (!enableResult.Succeeded)
        {
            var error = Uri.EscapeDataString(enableResult.GetError().Message);
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = provider.Name
            });

            return Results.Redirect(url);
        }

        var link = UrlGenerator.Url(returnUri, new { sessionId = session.Id, token });

        return Result.Success(link);
    }

    private async Task<Result> LockedOutAsync(UserEntity user, LockoutStateEntity lockoutState,
        OAuthProviderEntity provider, HttpContext context, string fallbackUri, CancellationToken cancellationToken)
    {
        var loginSessionResult = await loginSessionManager.CreateAsync(user, context,
            LoginStatus.Locked, LoginType.OAuth, provider.Name, cancellationToken);

        if (!loginSessionResult.Succeeded)
        {
            var error = Uri.EscapeDataString(loginSessionResult.GetError().Message);
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = provider.Name
            });

            return Results.Redirect(url);
        }
        else
        {
            var error = Uri.EscapeDataString($"This user account is locked out with reason: {lockoutState.Reason}.");
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = provider.Name
            });

            return Results.Redirect(url);
        }
    }

    private async Task<Result> UpdateSessionAsync(UserEntity user, OAuthSessionEntity session,
        OAuthProviderEntity provider, HttpContext context, string fallbackUri, CancellationToken cancellationToken)
    {
        var sessionResult = await sessionManager.UpdateAsync(session, cancellationToken);

        if (!sessionResult.Succeeded)
        {
            var loginSessionResult = await loginSessionManager.CreateAsync(user, context,
                LoginStatus.Failed, LoginType.OAuth, provider.Name, cancellationToken);

            if (!loginSessionResult.Succeeded)
            {
                var error = Uri.EscapeDataString(loginSessionResult.GetError().Message);
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.InternalError),
                    Message = error,
                    Provider = provider.Name
                });

                return Results.Redirect(url);
            }
            else
            {
                var error = Uri.EscapeDataString(sessionResult.GetError().Message);
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.InternalError),
                    Message = error,
                    Provider = provider.Name
                });

                return Results.Redirect(url);
            }
        }

        return Result.Success();
    }

    private async Task<Result> IsProviderAllowedAsync(UserEntity user, OAuthProviderEntity provider,
        HttpContext context, string fallbackUri, CancellationToken cancellationToken)
    {
        if (user.OAuthProviders.Any(x => x.ProviderId == provider.Id && !x.Allowed))
        {
            var loginSessionResult = await loginSessionManager.CreateAsync(user, context,
                LoginStatus.Failed, LoginType.OAuth, provider.Name, cancellationToken);

            if (!loginSessionResult.Succeeded)
            {
                var error = Uri.EscapeDataString(loginSessionResult.GetError().Message);
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.InternalError),
                    Message = error,
                    Provider = provider.Name
                });

                return Results.Redirect(url);
            }
            else
            {
                var error = Uri.EscapeDataString("OAuth provider is disallowed by user");
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.Unavailable),
                    Message = error,
                    Provider = provider.Name
                });

                return Results.Redirect(url);
            }
        }

        return Result.Success();
    }

    private async Task<Result> CheckUserProviderAsync(UserEntity user, OAuthProviderEntity provider,
        HttpContext context, string fallbackUri, CancellationToken cancellationToken)
    {
        var enableResult = await oAuthProviderManager.EnableAsync(user, provider, cancellationToken);

        if (!enableResult.Succeeded)
        {
            var loginSessionResult = await loginSessionManager.CreateAsync(user, context,
                LoginStatus.Failed, LoginType.OAuth, provider.Name, cancellationToken);

            if (!loginSessionResult.Succeeded)
            {
                var error = Uri.EscapeDataString(loginSessionResult.GetError().Message);
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.InternalError),
                    Message = error,
                    Provider = provider.Name
                });

                return Results.Redirect(url);
            }
            else
            {
                var error = Uri.EscapeDataString(enableResult.GetError().Message);
                var url = UrlGenerator.Url(fallbackUri, new
                {
                    ErrorCode = nameof(OAuthErrorType.InternalError),
                    Message = error,
                    Provider = provider.Name
                });

                return Results.Redirect(url);
            }
        }

        return Result.Success();
    }

    private async Task<Result> SignInAsync(UserEntity user, OAuthProviderEntity provider,
        OAuthSessionEntity session, HttpContext context, string returnUri, string fallbackUri,
        string token, CancellationToken cancellationToken)
    {
        var result = await loginSessionManager.CreateAsync(user, context,
            LoginStatus.Success, LoginType.OAuth, provider.Name, cancellationToken);

        if (!result.Succeeded)
        {
            var error = Uri.EscapeDataString(result.GetError().Message);
            var url = UrlGenerator.Url(fallbackUri, new
            {
                ErrorCode = nameof(OAuthErrorType.InternalError),
                Message = error,
                Provider = provider.Name
            });

            return Results.Redirect(url);
        }

        var link = UrlGenerator.Url(returnUri, new { sessionId = session.Id, token });

        return Result.Success(link);
    }
}