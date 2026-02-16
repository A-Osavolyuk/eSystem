using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Email.Commands;

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
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        var secondaryEmails = await _emailManager.GetAllAsync(user, EmailType.Secondary, cancellationToken);
        if (secondaryEmails.Count >= _options.SecondaryEmailMaxCount)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.MaxEmailsCount,
                Description = "User already has maximum count of secondary emails."
            });
        }

        if (_options.RequireUniqueEmail)
        {
            var taken = await _emailManager.IsTakenAsync(request.Request.Email, cancellationToken);
            if (taken) return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.EmailTaken,
                Description = "Email is already taken"
            });
        }

        var email = request.Request.Email;
        var result = await _emailManager.AddAsync(user, email, EmailType.Secondary, cancellationToken);
        
        return result;
    }
}