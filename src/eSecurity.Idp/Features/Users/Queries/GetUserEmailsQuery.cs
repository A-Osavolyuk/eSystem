using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.DTOs;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.Users.Queries;

public record GetUserEmailsQuery : IRequest<Result>;

public class GetUserEmailsQueryHandler(
    IUserManager userManager,
    IEmailManager emailManager) : IRequestHandler<GetUserEmailsQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;

    public async Task<Result> Handle(GetUserEmailsQuery request, CancellationToken cancellationToken)
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

        var emails = await _emailManager.GetAllAsync(user, cancellationToken);
        var response = emails.Select(email => new UserEmailDto
        {
            Id = email.Id,
            Email = email.Email,
            NormalizedEmail = email.NormalizedEmail,
            Type = email.Type,
            IsVerified = email.IsVerified,
            VerifiedAt = email.VerifiedAt,
            UpdatedAt = email.UpdatedAt
        });

        return Results.Success(SuccessCodes.Ok, response);
    }
}