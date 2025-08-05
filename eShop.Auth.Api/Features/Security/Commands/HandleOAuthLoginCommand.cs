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
    IOAuthProviderManager oAuthProviderManager,
    IOAuthSessionManager oAuthSessionManager) : IRequestHandler<HandleOAuthLoginCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IOAuthProviderManager oAuthProviderManager = oAuthProviderManager;
    private readonly IOAuthSessionManager oAuthSessionManager = oAuthSessionManager;

    public async Task<Result> Handle(HandleOAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        OAuthSessionEntity? session;
        string? link;

        var providerName = request.Principal.Identity!.AuthenticationType!;

        if (string.IsNullOrEmpty(providerName))
        {
            session = new OAuthSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                ErrorCode = ErrorCode.NotFound,
                ErrorMessage = "Cannot find provider",
                SignType = OAuthSignType.None,
                IsSucceeded = false,
                CreateDate = DateTimeOffset.UtcNow,
                ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            await oAuthSessionManager.CreateAsync(session, cancellationToken);

            link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
            return Result.Success(link);
        }

        var provider = await oAuthProviderManager.FindByNameAsync(providerName, cancellationToken);

        if (provider is null)
        {
            session = new OAuthSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                ErrorCode = ErrorCode.BadRequest,
                ErrorMessage = $"Provider {providerName} is not supported",
                SignType = OAuthSignType.None,
                IsSucceeded = false,
                CreateDate = DateTimeOffset.UtcNow,
                ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            await oAuthSessionManager.CreateAsync(session, cancellationToken);

            link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
            return Result.Success(link);
        }

        if (!string.IsNullOrEmpty(request.RemoteError))
        {
            session = new OAuthSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                ErrorCode = ErrorCode.InternalServerError,
                ErrorMessage = request.RemoteError,
                SignType = OAuthSignType.None,
                IsSucceeded = false,
                CreateDate = DateTimeOffset.UtcNow,
                ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            await oAuthSessionManager.CreateAsync(session, cancellationToken);

            link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
            return Result.Success(link);
        }

        var email = request.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null)
        {
            session = new OAuthSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                ErrorCode = ErrorCode.BadRequest,
                ErrorMessage = "Email was not provided in credentials",
                SignType = OAuthSignType.None,
                IsSucceeded = false,
                CreateDate = DateTimeOffset.UtcNow,
                ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            await oAuthSessionManager.CreateAsync(session, cancellationToken);

            link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
            return Result.Success(link);
        }

        var user = await userManager.FindByEmailAsync(email, cancellationToken);

        if (user is not null)
        {
            var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

            if (lockoutState.Enabled)
            {
                session = new OAuthSessionEntity()
                {
                    Id = Guid.CreateVersion7(),
                    ErrorCode = ErrorCode.BadRequest,
                    ErrorMessage = $"This user account is locked out with reason: {lockoutState.Reason}.",
                    SignType = OAuthSignType.None,
                    IsSucceeded = false,
                    CreateDate = DateTimeOffset.UtcNow,
                    ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
                };
            }
            else
            {
                session = new OAuthSessionEntity()
                {
                    Id = Guid.CreateVersion7(),
                    UserId = user.Id,
                    ProviderId = provider.Id,
                    SignType = OAuthSignType.SignIn,
                    IsSucceeded = true,
                    CreateDate = DateTimeOffset.UtcNow,
                    ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
                };
            }
        }
        else
        {
            user = new UserEntity()
            {
                Id = Guid.CreateVersion7(),
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, cancellationToken);

            if (!result.Succeeded)
            {
                session = new OAuthSessionEntity()
                {
                    Id = Guid.CreateVersion7(),
                    ErrorCode = result.GetError().Code,
                    ErrorMessage = result.GetError().Message,
                    SignType = OAuthSignType.None,
                    IsSucceeded = false,
                    CreateDate = DateTimeOffset.UtcNow,
                    ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
                };

                await oAuthSessionManager.CreateAsync(session, cancellationToken);

                link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
                return Result.Success(link);
            }

            var role = await roleManager.FindByNameAsync("User", cancellationToken);

            if (role is null)
            {
                session = new OAuthSessionEntity()
                {
                    Id = Guid.CreateVersion7(),
                    ErrorCode = ErrorCode.NotFound,
                    ErrorMessage = "Cannot find role with name User",
                    SignType = OAuthSignType.None,
                    IsSucceeded = false,
                    CreateDate = DateTimeOffset.UtcNow,
                    ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
                };

                await oAuthSessionManager.CreateAsync(session, cancellationToken);

                link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
                return Result.Success(link);
            }

            var assignRoleResult = await roleManager.AssignAsync(user, role, cancellationToken);

            if (!assignRoleResult.Succeeded)
            {
                session = new OAuthSessionEntity()
                {
                    Id = Guid.CreateVersion7(),
                    ErrorCode = assignRoleResult.GetError().Code,
                    ErrorMessage = assignRoleResult.GetError().Message,
                    SignType = OAuthSignType.None,
                    IsSucceeded = false,
                    CreateDate = DateTimeOffset.UtcNow,
                    ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
                };

                await oAuthSessionManager.CreateAsync(session, cancellationToken);

                link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
                return Result.Success(link);
            }

            if (role.Permissions.Count > 0)
            {
                var permissions = role.Permissions.Select(x => x.Permission).ToList();

                foreach (var permission in permissions)
                {
                    var grantResult = await permissionManager.GrantAsync(user, permission, cancellationToken);

                    if (grantResult.Succeeded) continue;

                    session = new OAuthSessionEntity()
                    {
                        Id = Guid.CreateVersion7(),
                        ErrorCode = grantResult.GetError().Code,
                        ErrorMessage = grantResult.GetError().Message,
                        SignType = OAuthSignType.None,
                        IsSucceeded = false,
                        CreateDate = DateTimeOffset.UtcNow,
                        ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
                    };

                    await oAuthSessionManager.CreateAsync(session, cancellationToken);

                    link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
                    return Result.Success(link);
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

            session = new OAuthSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ProviderId = provider.Id,
                SignType = OAuthSignType.SignUp,
                IsSucceeded = true,
                CreateDate = DateTimeOffset.UtcNow,
                ExpiredDate = DateTimeOffset.UtcNow.AddMinutes(10)
            };
        }

        await oAuthSessionManager.CreateAsync(session, cancellationToken);
        link = UrlGenerator.ActionLink(request.ReturnUri!, new { sessionId = session.Id });
        return Result.Success(link);
    }
}