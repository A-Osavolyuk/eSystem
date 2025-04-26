using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyPhoneNumberCommand(VerifyPhoneNumberRequest Request)
    : IRequest<Result>;

internal sealed class VerifyPhoneNumberCommandHandler(
    ISecurityManager securityManager,
    UserManager<UserEntity> userManager) : IRequestHandler<VerifyPhoneNumberCommand, Result>
{
    private readonly ISecurityManager securityManager = securityManager;
    private readonly UserManager<UserEntity> userManager = userManager;

    public async Task<Result> Handle(VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByPhoneNumberAsync(request.Request.PhoneNumber);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with phone number ${request.Request.PhoneNumber}");
        }

        var result = await securityManager.VerifyPhoneNumberAsync(user, request.Request.Code);

        if (!result.Succeeded)
        {
            return result;
        }

        return Result.Success("Phone number was successfully verified");
    }
}