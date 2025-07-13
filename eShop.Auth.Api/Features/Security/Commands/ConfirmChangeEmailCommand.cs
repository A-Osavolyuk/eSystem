using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ConfirmChangeEmailCommand(ConfirmChangeEmailRequest Request)
    : IRequest<Result>;

public sealed class ConfirmChangeEmailCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmChangeEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var currentEmailResult = await codeManager.VerifyAsync(user, request.Request.CurrentEmailCode, 
            SenderType.Email, CodeType.Current, CodeResource.Email, cancellationToken);

        if (!currentEmailResult.Succeeded)
        {
            return currentEmailResult;
        }
        
        var newEmailResult = await codeManager.VerifyAsync(user, request.Request.NewEmailCode, 
            SenderType.Email, CodeType.New, CodeResource.Email, cancellationToken);
        
        if (!newEmailResult.Succeeded)
        {
            return newEmailResult;
        }
        
        var result = await userManager.ChangeEmailAsync(user, request.Request.NewEmail, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }
        
        user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        
        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        var response = new ConfirmChangeEmailResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        return Result.Success(response);
    }
}