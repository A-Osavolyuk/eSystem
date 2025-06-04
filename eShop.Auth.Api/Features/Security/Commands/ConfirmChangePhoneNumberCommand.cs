using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ConfirmChangePhoneNumberCommand(ConfirmPhoneNumberChangeRequest Request)
    : IRequest<Result>;

internal sealed class ConfirmChangePhoneNumberCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager)
    : IRequestHandler<ConfirmChangePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;

    public async Task<Result> Handle(ConfirmChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByPhoneNumberAsync(request.Request.CurrentPhoneNumber, cancellationToken);

        if (user is null)
        {
            return Results.InternalServerError(
                $"Cannot find user with phone number {request.Request.CurrentPhoneNumber}.");
        }
        
        var currentPhoneNumberCode = request.Request.CurrentPhoneNumber;
        var newPhoneNumberCode = request.Request.NewPhoneNumberCode;

        var result = await userManager.ChangePhoneNumberAsync(user, currentPhoneNumberCode, newPhoneNumberCode, 
            request.Request.NewPhoneNumber, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user = await userManager.FindByPhoneNumberAsync(request.Request.NewPhoneNumber, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with phone number {request.Request.NewPhoneNumber}.");
        }
        
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        return Result.Success(new ConfirmChangePhoneNumberResponse()
        {
            Message = "Your phone number was successfully changed.",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        });
    }
}