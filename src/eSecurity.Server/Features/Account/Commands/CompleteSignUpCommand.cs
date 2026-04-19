using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authorization.Codes;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Features.Account.Commands;

public sealed record CompleteSignUpCommand(CompleteSignUpRequest Request) : IRequest<Result>;

public sealed class CompleteSignUpCommandHandler(
    IAuthenticationSessionManager authenticationSessionManager,
    ISessionManager sessionManager,
    IUserManager userManager,
    IEmailManager emailManager,
    ICodeManager codeManager,
    IOptions<SessionOptions> options,
    ISessionCookieFactory sessionCookieFactory) : IRequestHandler<CompleteSignUpCommand, Result>
{
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly ISessionCookieFactory _sessionCookieFactory = sessionCookieFactory;
    private readonly SessionOptions _options = options.Value;

    public async Task<Result> Handle(CompleteSignUpCommand request, CancellationToken cancellationToken = default)
    {
        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(
            request.Request.TransactionId, cancellationToken);

        if (authenticationSession is null || !authenticationSession.IsActive || authenticationSession.UserId is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidSession,
                Description = "Invalid session"
            });
        }

        var user = await _userManager.FindByIdAsync(authenticationSession.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

        var code = await _codeManager.FindAsync(user, request.Request.Code, cancellationToken);
        if (code is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound, 
                Description = "Code not found"
            });
        }

        var codeResult = await _codeManager.ConsumeAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var primaryEmail = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (primaryEmail is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Email not found"
            });
        }

        var emailResult = await _emailManager.VerifyAsync(user, primaryEmail.Email, cancellationToken);
        if (!emailResult.Succeeded) return emailResult;

        var session = new SessionEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            ExpireDate = DateTimeOffset.UtcNow.Add(_options.Timestamp)
        };

        session.AddMethods(AuthenticationMethodReference.EmailBasedAuthentication);
        await _sessionManager.CreateAsync(session, cancellationToken);

        authenticationSession.SessionId = session.Id;
        authenticationSession.Pass(AuthenticationMethodReference.EmailBasedAuthentication);

        var authenticationSessionResult = await _authenticationSessionManager.UpdateAsync(
            authenticationSession, cancellationToken);

        if (!authenticationSessionResult.Succeeded) 
            return authenticationSessionResult;
        
        return Results.Success(SuccessCodes.Ok, new CompleteSignUpResponse
        {
            SessionCookie = _sessionCookieFactory.CreateCookie(session)
        });
    }
}