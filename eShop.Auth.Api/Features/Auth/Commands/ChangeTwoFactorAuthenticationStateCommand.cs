namespace eShop.Auth.Api.Features.Auth.Commands;

internal sealed record ChangeTwoFactorAuthenticationStateCommand(ChangeTwoFactorAuthenticationRequest Request)
    : IRequest<Result<ChangeTwoFactorAuthenticationResponse>>;

internal sealed class ChangeTwoFactorAuthenticationStateCommandHandler(
    AppManager appManager)
    : IRequestHandler<ChangeTwoFactorAuthenticationStateCommand, Result<ChangeTwoFactorAuthenticationResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<ChangeTwoFactorAuthenticationResponse>> Handle(
        ChangeTwoFactorAuthenticationStateCommand request, CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email {request.Request.Email}."));
        }

        IdentityResult result = null!;

        result = await appManager.UserManager.SetTwoFactorEnabledAsync(user, !user.TwoFactorEnabled);

        if (!result.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot change 2fa state of user with email {request.Request.Email} " +
                $"due to server error: {result.Errors.First().Description}."));
        }

        var state = user.TwoFactorEnabled ? "disabled" : "enabled";

        return new(new ChangeTwoFactorAuthenticationResponse()
        {
            Message = $"Two factor authentication was successfully {state}.",
            TwoFactorAuthenticationState = user.TwoFactorEnabled,
        });
    }
}