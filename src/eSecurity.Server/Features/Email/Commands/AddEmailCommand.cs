using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Email.Commands;

public record AddEmailCommand(AddEmailRequest Request) : IRequest<Result>;

public class AddEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AccountOptions> options) : IRequestHandler<AddEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(AddEmailCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.ClientError(ClientErrorCode.BadRequest, new Error()
        {
            Code = ErrorCode.BadRequest,
            Description = "Invalid request"
        });
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

        var secondaryEmails = await _emailManager.GetAllAsync(user, EmailType.Secondary, cancellationToken);
        if (secondaryEmails.Count >= _options.SecondaryEmailMaxCount)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.MaxEmailsCount,
                Description = "User already has maximum count of secondary emails."
            });
        }

        if (_options.RequireUniqueEmail)
        {
            var taken = await _emailManager.IsTakenAsync(request.Request.Email, cancellationToken);
            if (taken) return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.EmailTaken,
                Description = "Email is already taken"
            });
        }

        var email = request.Request.Email;
        var result = await _emailManager.AddAsync(user, email, EmailType.Secondary, cancellationToken);
        
        return result;
    }
}