using System.Security.Claims;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Identity.Claims;
using eSystem.Core.Security.Identity.Email;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSecurity.Features.Users.Queries;

public record GetUserClaimsQuery(Guid UserId) : IRequest<Result>;

public class GetUserClaimsQueryHandler(IUserManager userManager) : IRequestHandler<GetUserClaimsQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserClaimsQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"User with ID {request.UserId} was not found.");

        List<Claim> claims = [
            new(AppClaimTypes.Sub, user.Id.ToString()),
            new(AppClaimTypes.PreferredUsername, user.Username)
        ];
        
        claims.AddRange(user.Roles.Select(
            x => new Claim(AppClaimTypes.Role, x.Role.Name)));
        
        claims.AddRange(user.Permissions.Select(
            x => new Claim(AppClaimTypes.Permission, x.Permission.Name)));

        var email = user.GetEmail(EmailType.Primary);
        if (email is not null)
        {
            claims.Add(new Claim(AppClaimTypes.Email, email.Email));
            claims.Add(new Claim(AppClaimTypes.EmailVerified, email.IsVerified.ToString()));
        }

        var phoneNumber = user.GetPhoneNumber(PhoneNumberType.Primary);
        if (phoneNumber is not null)
        {
            claims.Add(new Claim(AppClaimTypes.PhoneNumber, phoneNumber.PhoneNumber));
            claims.Add(new Claim(AppClaimTypes.PhoneNumberVerified, phoneNumber.IsVerified.ToString()));
        }

        return Result.Success(claims);
    }
}