using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmChangePhoneNumberCommand(ConfirmChangePhoneNumberRequest Request)
    : IRequest<Result>;

internal sealed class ConfirmChangePhoneNumberCommandHandler(
    UserManager<UserEntity> userManager,
    ISecurityManager securityManager,
    ITokenManager tokenManager)
    : IRequestHandler<ConfirmChangePhoneNumberCommand, Result>
{
    private readonly ISecurityManager securityManager = securityManager;
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;

    public async Task<Result> Handle(ConfirmChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByPhoneNumberAsync(request.Request.CurrentPhoneNumber);

        if (user is null)
        {
            return Results.InternalServerError(
                $"Cannot find user with phone number {request.Request.CurrentPhoneNumber}.");
        }

        var result =
            await securityManager.ChangePhoneNumberAsync(user, request.Request.NewPhoneNumber,
                request.Request.CodeSet);

        if (!result.Succeeded)
        {
            return result;
        }

        user = await userManager.FindByPhoneNumberAsync(request.Request.NewPhoneNumber);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with phone number {request.Request.NewPhoneNumber}.");
        }
        
        var tokens = await tokenManager.GenerateAsync(user, cancellationToken);

        return Result.Success(new ConfirmChangePhoneNumberResponse()
        {
            Message = "Your phone number was successfully changed.",
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        });
    }
}