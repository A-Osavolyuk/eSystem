using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserSecurityQuery(Guid UserId) : IRequest<Result>;

public class GetUserSecurityQueryHandler(IUserManager userManager) : IRequestHandler<GetUserSecurityQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserSecurityQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = new UserSecurityDto()
        {
            UserId = user.Id,
            RecoveryEmail = user.RecoveryEmail?.Email,
            RecoveryEmailConfirmed = user.RecoveryEmail?.IsVerified,
            RecoveryEmailChangeDate = user.RecoveryEmail?.UpdateDate,
            RecoveryEmailConfirmationDate = user.RecoveryEmail?.VerifiedDate,
            HasPassword = !string.IsNullOrEmpty(user.PasswordHash),
            PasswordChangeDate = user.PasswordChangeDate,
            TwoFactorEnabled = user.HasTwoFactor(),
            Providers = user.TwoFactorProviders.Select(Mapper.Map).ToList(),
            HasLinkedAccounts = user.LinkedAccounts.Count > 0,
            OAuthProviders = user.LinkedAccounts.Select(Mapper.Map).ToList(),
            Devices = user.Devices.Select(Mapper.Map).ToList(),
            Passkeys = user.Passkeys.Select(Mapper.Map).ToList()
        };
        
        return Result.Success(response);
    }
}