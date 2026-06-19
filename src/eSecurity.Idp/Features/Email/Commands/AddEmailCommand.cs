using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Options;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Identity;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Commands;

public record AddEmailCommand(AddEmailRequest Request) : IRequest<Result>;

public class AddEmailCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService,
    IEmailCommandService emailCommandService,
    IOptions<AccountOptions> options) : IRequestHandler<AddEmailCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(AddEmailCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
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

        var secondaryEmails = await _emailQueryService.ListByTypeAsync(
            user.Id, EmailType.Secondary, cancellationToken);
        
        if (secondaryEmails.Count >= _options.SecondaryEmailMaxCount)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.MaxEmailsCount,
                Description = "User already has maximum count of secondary emails."
            });
        }

        if (await _emailQueryService.ExistsAsync(request.Request.Email, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.EmailTaken,
                Description = "Email is already taken"
            });
        }

        var email = request.Request.Email;
        var result = await _emailCommandService.AddAsync(user.Id, email, EmailType.Secondary, cancellationToken);
        
        return result;
    }
}