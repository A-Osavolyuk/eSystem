using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmChangePhoneNumberCommand(ConfirmChangePhoneNumberRequest Request)
    : IRequest<Result>;

internal sealed class ConfirmChangePhoneNumberCommandHandler(
    IUserManager userManager,
    ISecurityManager securityManager,
    ISecurityTokenManager securityTokenManager)
    : IRequestHandler<ConfirmChangePhoneNumberCommand, Result>
{
    private readonly ISecurityManager securityManager = securityManager;
    private readonly IUserManager userManager = userManager;
    private readonly ISecurityTokenManager securityTokenManager = securityTokenManager;

    public async Task<Result> Handle(ConfirmChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByPhoneNumberAsync(request.Request.CurrentPhoneNumber, cancellationToken);

        if (user is null)
        {
            return Results.InternalServerError(
                $"Cannot find user with phone number {request.Request.CurrentPhoneNumber}.");
        }

        var result = await securityManager.ChangePhoneNumberAsync(user, request.Request.NewPhoneNumber, request.Request.CodeSet);

        if (!result.Succeeded)
        {
            return result;
        }

        user = await userManager.FindByPhoneNumberAsync(request.Request.NewPhoneNumber, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with phone number {request.Request.NewPhoneNumber}.");
        }
        
        var tokens = await securityTokenManager.GenerateAsync(user, cancellationToken);

        return Result.Success(new ConfirmChangePhoneNumberResponse()
        {
            Message = "Your phone number was successfully changed.",
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        });
    }
}