using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserPrimaryEmailQuery(Guid UserId) : IRequest<Result>;

public class GetUserPrimaryEmailQueryHandler(
    IUserManager userManager,
    IEmailManager emailManager) : IRequestHandler<GetUserPrimaryEmailQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;

    public async Task<Result> Handle(GetUserPrimaryEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID: {request.UserId}.");

        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (email is null) return Results.BadRequest("User does not have a primary email.");

        var response = new UserEmailDto()
        {
            Id = email.Id,
            Type = email.Type,
            Email = email.Email,
            NormalizedEmail = email.NormalizedEmail,
            IsVerified = email.IsVerified,
            UpdateDate = email.UpdateDate,
            VerifiedDate = email.VerifiedDate
        };

        return Results.Ok(response);
    }
}