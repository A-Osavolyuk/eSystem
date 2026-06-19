using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.DTOs;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.Users.Queries;

public record GetUserEmailsQuery : IRequest<Result>;

public class GetUserEmailsQueryHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService) : IRequestHandler<GetUserEmailsQuery, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;

    public async Task<Result> Handle(GetUserEmailsQuery request, CancellationToken cancellationToken)
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

        var emails = await _emailQueryService.ListByUserAsync(user.Id, cancellationToken);
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