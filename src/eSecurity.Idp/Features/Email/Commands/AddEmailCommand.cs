using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Options;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Identity;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.Email.Commands;

public record AddEmailCommand(AddEmailRequest Request) : IRequest<Result>;

public class AddEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IOptions<AccountOptions> options) : IRequestHandler<AddEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(AddEmailCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userManager.GetUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
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