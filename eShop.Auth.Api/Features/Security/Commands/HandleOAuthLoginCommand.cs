using System.Security.Claims;
using eShop.Auth.Api.Messages.Email;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record HandleOAuthLoginCommand(
    ClaimsPrincipal Principal,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

public sealed class HandleOAuthLoginCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IMessageService messageService,
    IRoleManager roleManager,
    ILockoutManager lockoutManager,
    IOAuthProviderManager providerManager,
    IOAuthSessionManager sessionManager) : IRequestHandler<HandleOAuthLoginCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;

    public async Task<Result> Handle(HandleOAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.RemoteError))
        {
            return await FailAsync(OAuthErrorType.RemoteError, request.RemoteError, request.ReturnUri!, cancellationToken);
        }

        var providerName = request.Principal.Identity!.AuthenticationType!;
        var provider = await providerManager.FindByNameAsync(providerName, cancellationToken);

        if (provider is null)
        {
            return await FailAsync(OAuthErrorType.UnsupportedProvider, 
                $"Provider {providerName} is not supported", request.ReturnUri!, cancellationToken);
        }

        var email = request.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null)
        {
            return await FailAsync(OAuthErrorType.InvalidCredentials, 
                "Email was not provided in credentials", request.ReturnUri!, cancellationToken);
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
                return await FailAsync(OAuthErrorType.InternalError, 
                    createResult.GetError().Message, request.ReturnUri!, cancellationToken);
            }

            var role = await roleManager.FindByNameAsync("User", cancellationToken);

            if (role is null)
            {
                return await FailAsync(OAuthErrorType.InternalError, "Cannot find role with name User", 
                    request.ReturnUri!, cancellationToken);
            }

            var assignRoleResult = await roleManager.AssignAsync(user, role, cancellationToken);

            if (!assignRoleResult.Succeeded)
            {
                return await FailAsync(OAuthErrorType.InternalError, 
                    createResult.GetError().Message, request.ReturnUri!, cancellationToken);
            }

            if (role.Permissions.Count > 0)
            {
                var permissions = role.Permissions.Select(x => x.Permission).ToList();

                foreach (var permission in permissions)
                {
                    var result = await permissionManager.GrantAsync(user, permission, cancellationToken);

                    if (result.Succeeded) continue;

                    return await FailAsync(OAuthErrorType.InternalError, 
                        createResult.GetError().Message, request.ReturnUri!, cancellationToken);
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

            var session = new OAuthSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ProviderId = provider.Id,
                SignType = OAuthSignType.SignUp,
                IsSucceeded = true,
                CreateDate = DateTimeOffset.UtcNow,
                ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
            };
            
            await sessionManager.CreateAsync(session, cancellationToken);
            var link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
            return Result.Success(link);
        }
        else
        {
            var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

            if (lockoutState.Enabled)
            {
                return await FailAsync(OAuthErrorType.InternalError, 
                    $"This user account is locked out with reason: {lockoutState.Reason}.", 
                    request.ReturnUri!, cancellationToken);
            }

            var session = new OAuthSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ProviderId = provider.Id,
                SignType = OAuthSignType.SignIn,
                IsSucceeded = true,
                CreateDate = DateTimeOffset.UtcNow,
                ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
            };
        
            await sessionManager.CreateAsync(session, cancellationToken);
            var link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
            return Result.Success(link);
        }
    }

    private async Task<Result> FailAsync(OAuthErrorType errorType, string errorMessage, 
        string returnUri, CancellationToken cancellationToken = default)
    {
        var session = new OAuthSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            ErrorType = errorType,
            ErrorMessage = errorMessage,
            SignType = OAuthSignType.None,
            IsSucceeded = false,
            CreateDate = DateTimeOffset.UtcNow,
            ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
        };
        
        await sessionManager.CreateAsync(session, cancellationToken);
        var link = UrlGenerator.ActionLink(returnUri, new { sessionId = session.Id });
        return Result.Success(link);
    }
}