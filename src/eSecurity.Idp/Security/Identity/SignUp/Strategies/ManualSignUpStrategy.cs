using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Authorization.Roles;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Options;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Idp.Security.Identity.User.Username;
using eSecurity.Core.Responses;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Security.Authentication.AuthenticationSession;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Idp.Security.Identity.SignUp.Strategies;

public sealed class ManualSignUpPayload : SignUpPayload
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}

public sealed class ManualSignUpStrategy(
    IUsernameManager usernameManager,
    IPasswordManager passwordManager,
    IRoleManager roleManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailService emailService,
    IEmailQueryService emailQueryService,
    IEmailCommandService emailCommandService,
    IAuthenticationSessionManager sessionManager,
    IUserCommandService userCommandService,
    ICodeCommandService codeCommandService,
    IOptions<AccountOptions> options) : ISignUpStrategy
{
    private readonly IUsernameManager _usernameManager = usernameManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly IRoleManager _roleManager = roleManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly IAuthenticationSessionManager _sessionManager = sessionManager;
    private readonly IUserCommandService _userCommandService = userCommandService;
    private readonly ICodeCommandService _codeCommandService = codeCommandService;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly AccountOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(SignUpPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not ManualSignUpPayload manualPayload)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Incorrect payload type"
            });
        }
        
        if (await _usernameManager.IsTakenAsync(manualPayload.Username, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UsernameTaken,
                Description = "This username is already taken"
            });
        }

        if (await _emailQueryService.ExistsAsync(manualPayload.Email, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.EmailTaken,
                Description = "This email is already taken."
            });
        }

        var user = new UserEntity(manualPayload.Username);
        var createResult = await _userCommandService.CreateAsync(user, cancellationToken);
        if (!createResult.Succeeded) return createResult;

        var passwordResult = await _passwordManager.AddAsync(user, manualPayload.Password, cancellationToken);
        if (!passwordResult.Succeeded) return passwordResult;

        var setResult = await _emailCommandService.AddAsync(user.Id, 
            manualPayload.Email, EmailType.Primary, cancellationToken);

        if (!setResult.Succeeded) return setResult;

        var role = await _roleManager.FindByNameAsync("User", cancellationToken);
        if (role is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Cannot find role with name User"
            });
        }

        var assignRoleResult = await _roleManager.AssignAsync(user, role, cancellationToken);
        if (!assignRoleResult.Succeeded) return assignRoleResult;

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var clientInfo = _httpContext.GetClientInfo();

        var newDevice = new UserDeviceEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            Browser = clientInfo.UA.ToString(),
            Os = clientInfo.OS.ToString(),
            Device = clientInfo.Device.ToString(),
            IsBlocked = false,
            FirstSeenAt = DateTimeOffset.UtcNow
        };

        var deviceResult = await _deviceManager.CreateAsync(newDevice, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;

        var codeResult = await _codeCommandService.CreateAsync(user.Id, SenderType.Email, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
            return Results.ServerError(ServerErrorCode.InternalServerError, error);
        }

        if (!codeResult.TryGetValue(out var code))
        {
            return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
        var emailContext = new CodeVerificationEmailContext
        {
            Subject = "Sign Up",
            To = manualPayload.Email,
            Code = code
        };

        await _emailService.SendAsync(emailContext, cancellationToken);

        var session = new AuthenticationSessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(10)
        };
        
        session.Require(AuthenticationMethodReference.EmailBasedAuthentication);
        
        var sessionResult = await _sessionManager.CreateAsync(session, cancellationToken);
        if (!sessionResult.Succeeded) return sessionResult;

        return Results.Success(SuccessCodes.Ok, new SignUpResponse
        {
            TransactionId = session.Id
        });
    }
}