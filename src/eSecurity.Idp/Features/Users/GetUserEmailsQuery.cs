using eSecurity.Core.DTOs;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public record GetUserEmailsQuery : IRequest<Result>;

public class GetUserEmailsQueryHandler(
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService) : IRequestHandler<GetUserEmailsQuery, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;

    public async Task<Result> Handle(GetUserEmailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
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