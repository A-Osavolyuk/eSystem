using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ConfirmChangeEmailCommand(ConfirmChangeEmailRequest Request)
    : IRequest<Result>;

public sealed class ConfirmChangeEmailCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    ICodeManager codeManager,
    IRollbackManager rollbackManager,
    IMessageService messageService) : IRequestHandler<ConfirmChangeEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IRollbackManager rollbackManager = rollbackManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(ConfirmChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var isTaken = await userManager.CheckEmailAsync(request.Request.NewEmail, cancellationToken);

        if (isTaken)
        {
            return Results.BadRequest("This email address is already taken");
        }
        
        var codeResult = await codeManager.VerifyAsync(user, request.Request.NewEmailCode, 
            SenderType.Email, CodeType.New, CodeResource.Email, cancellationToken);
        
        if (!codeResult.Succeeded)
        {
            return codeResult;
        }
        
        var rollback = await rollbackManager.CommitAsync(user, user.Email, RollbackField.Email, cancellationToken);

        if (rollback is null)
        {
            return Results.InternalServerError("Cannot change email, rollback was not created.");
        }
        
        var result = await userManager.ChangeEmailAsync(user, request.Request.NewEmail, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        var message = new EmailChangedMessage()
        {
            Credentials = new()
            {
                { "Subject", "Email changed" },
                { "To", rollback.Value }
            },
            Payload = new()
            {
                { "UserName", rollback.Value },
                { "Code", rollback.Code },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);
        
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