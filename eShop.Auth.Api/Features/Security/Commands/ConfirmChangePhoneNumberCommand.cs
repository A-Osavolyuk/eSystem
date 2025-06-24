using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ConfirmChangePhoneNumberCommand(ConfirmChangePhoneNumberRequest Request) : IRequest<Result>;

public sealed class ConfirmChangePhoneNumberCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    ICodeManager codeManager)
    : IRequestHandler<ConfirmChangePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var currentPhoneNumberCode = request.Request.CurrentPhoneNumberCode;
        var newPhoneNumberCode = request.Request.NewPhoneNumberCode;
        
        var currentPhoneNumberResult = await codeManager.VerifyAsync(user, currentPhoneNumberCode, 
            SenderType.Sms, CodeType.Current, cancellationToken);

        if (!currentPhoneNumberResult.Succeeded)
        {
            return currentPhoneNumberResult;
        }
        
        var newPhoneNumberResult = await codeManager.VerifyAsync(user, newPhoneNumberCode, 
            SenderType.Sms, CodeType.New, cancellationToken);

        if (!newPhoneNumberResult.Succeeded)
        {
            return newPhoneNumberResult;
        }

        var result = await userManager.ChangePhoneNumberAsync(user, request.Request.NewPhoneNumber, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

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