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
            RecoveryEmail = user.RecoveryEmail,
            RecoveryEmailConfirmed = user.RecoveryEmailConfirmed,
            RecoveryEmailChangeDate = user.RecoveryEmailChangeDate,
            RecoveryEmailConfirmationDate = user.RecoveryEmailConfirmationDate,
            HasPassword = !string.IsNullOrEmpty(user.PasswordHash),
            PasswordChangeDate = user.PasswordChangeDate,
            TwoFactorEnabled = user.Providers.Any(x => x.Subscribed),
            Providers = user.Providers.Select(Mapper.Map).ToList(),
            HasLinkedAccounts = user.OAuthProviders.Count > 0,
            OAuthProviders = user.OAuthProviders.Select(Mapper.Map).ToList(),
            Devices = user.Devices.Select(Mapper.Map).ToList(),
        };
        
        return Result.Success(response);
    }
}