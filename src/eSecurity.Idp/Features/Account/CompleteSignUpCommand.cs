using System.Text.Json.Serialization;
using eSecurity.Core.Responses;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.Account;

public sealed class CompleteSignUpCommand : IRequest<Result>
{
    [JsonPropertyName("transaction_id")]
    public Guid TransactionId { get; set; }
    
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}

public sealed class CompleteSignUpCommandHandler(
    IAuthenticationSessionManager authenticationSessionManager,
    ISessionManager sessionManager,
    ICodeManager codeManager,
    IEmailQueryService emailQueryService,
    IEmailCommandService emailCommandService,
    IOptions<SessionOptions> options,
    IUserQueryService userQueryService,
    ISessionCookieFactory sessionCookieFactory) : IRequestHandler<CompleteSignUpCommand, Result>
{
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ISessionCookieFactory _sessionCookieFactory = sessionCookieFactory;
    private readonly SessionOptions _options = options.Value;

    public async Task<Result> Handle(CompleteSignUpCommand request, CancellationToken cancellationToken = default)
    {
        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(
            request.TransactionId, cancellationToken);

        if (authenticationSession is null || !authenticationSession.IsActive || authenticationSession.UserId is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidSession,
                Description = "Invalid session"
            });
        }

        var user = await _userQueryService.GetByIdAsync(authenticationSession.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ValidationException("UserCode is required");
        
        var code = await _codeManager.FindByCodeAsync(user, request.Code, cancellationToken);
        if (code is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound, 
                Description = "Code not found"
            });
        }

        var codeResult = await _codeManager.ConsumeAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var primaryEmail = await _emailQueryService.GetByTypeAsync(user.Id, EmailType.Primary, cancellationToken);
        if (primaryEmail is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Email not found"
            });
        }

        var emailResult = await _emailCommandService.VerifyAsync(user.Id, primaryEmail.Email, cancellationToken);
        if (!emailResult.Succeeded) return emailResult;

        var session = new SessionEntity
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

public sealed class CompleteSignUpCommandValidator : IRequestValidator<CompleteSignUpCommand>
{
    public async ValueTask<Result> Validate(CompleteSignUpCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'code' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}