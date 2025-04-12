using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record Change2FaStateCommand(Change2FaStateRequest Request)
    : IRequest<Result>;

internal sealed class ChangeTwoFactorAuthenticationStateCommandHandler(
    AppManager appManager)
    : IRequestHandler<Change2FaStateCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(
        Change2FaStateCommand request, CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found.",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        IdentityResult result = null!;

        result = await appManager.UserManager.SetTwoFactorEnabledAsync(user, !user.TwoFactorEnabled);

        if (!result.Succeeded)
        {
            Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found.",
                Details = $"Cannot change 2fa state of user with email {request.Request.Email} " +
                          $"due to server error: {result.Errors.First().Description}."
            });
        }

        var state = user.TwoFactorEnabled ? "disabled" : "enabled";

        return Result.Success(new Change2FaStateResponse()
        {
            Message = $"Two factor authentication was successfully {state}.",
            TwoFactorAuthenticationState = user.TwoFactorEnabled,
        });
    }
}