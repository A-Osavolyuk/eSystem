using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Authorization.Verification;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Email.Commands;

public sealed record ChangeEmailCommand(ChangeEmailRequest Request) : IRequest<Result>;

public sealed class RequestChangeEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    ILinkedAccountManager linkedAccountManager,
    IVerificationManager verificationManager,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AccountOptions> options) : IRequestHandler<ChangeEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(ChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid request"
            });
        }

        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null)
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });

        if (request.Request.Type is EmailType.Secondary)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidEmail,
                Description = "Cannot change a secondary phone number."
            });
        }

        var currentEmail = await _emailManager.FindByTypeAsync(user, request.Request.Type, cancellationToken);
        if (currentEmail is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidEmail,
                Description = "User's primary email address is missing"
            });
        }

        if (_options.RequireUniqueEmail)
        {
            var isTaken = await _emailManager.IsTakenAsync(request.Request.Email, cancellationToken);
            if (isTaken)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.EmailTaken,
                    Description = "Email address is already taken"
                });
            }
        }

        if (await _linkedAccountManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.LinkedAccountConnected,
                Description = "Cannot change email, first disconnect linked accounts."
            });
        }

        var verification = await _verificationManager.FindByIdAsync(request.Request.VerificationId, cancellationToken);
        if (verification?.Status is not VerificationStatus.Approved)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Unverified request."
            });
        }

        var verificationResult = await _verificationManager.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var result = await _emailManager.ChangeAsync(user, currentEmail.Email,
            request.Request.Email, cancellationToken);

        return result;
    }
}