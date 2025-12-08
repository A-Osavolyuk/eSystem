using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public sealed record GetUserQuery(Guid UserId) : IRequest<Result>;

public sealed class GetUserQueryHandler(
    IUserManager userManager,
    IEmailManager emailManager) : IRequestHandler<GetUserQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;

    public async Task<Result> Handle(GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        var primaryPhoneNumber = user.GetPhoneNumber(PhoneNumberType.Primary);

        var response = new UserDto
        {
            Id = user.Id,
            Email = email?.Email,
            EmailConfirmed = email?.IsVerified,
            EmailChangeDate = email?.UpdateDate,
            EmailConfirmationDate = email?.VerifiedDate,
            PhoneNumber = primaryPhoneNumber?.PhoneNumber,
            PhoneNumberConfirmed = primaryPhoneNumber?.IsVerified,
            PhoneNumberChangeDate = primaryPhoneNumber?.UpdateDate,
            PhoneNumberConfirmationDate = primaryPhoneNumber?.VerifiedDate,
            Username = user.Username,
            UserNameChangeDate = user.UsernameChangeDate,
        };
        
        return Results.Ok(response);
    }
}