using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record Change2FaStateCommand(Change2FaStateRequest Request)
    : IRequest<Result>;

internal sealed class ChangeTwoFactorAuthenticationStateCommandHandler(
    UserManager<UserEntity> userManager)
    : IRequestHandler<Change2FaStateCommand, Result>
{
    private readonly UserManager<UserEntity> userManager = userManager;

    public async Task<Result> Handle(
        Change2FaStateCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var result = await userManager.SetTwoFactorEnabledAsync(user, !user.TwoFactorEnabled);

        if (!result.Succeeded)
        {
            Results.NotFound($"Cannot change 2fa state of user with email {request.Request.Email} " +
                             $"due to server error: {result.Errors.First().Description}.");
        }

        var state = user.TwoFactorEnabled ? "disabled" : "enabled";

        return Result.Success(new Change2FaStateResponse()
        {
            Message = $"Two factor authentication was successfully {state}.",
            TwoFactorAuthenticationState = user.TwoFactorEnabled,
        });
    }
}