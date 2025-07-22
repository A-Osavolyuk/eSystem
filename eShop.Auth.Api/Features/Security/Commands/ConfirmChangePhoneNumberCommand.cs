using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ConfirmChangePhoneNumberCommand(ConfirmChangePhoneNumberRequest Request) : IRequest<Result>;

public sealed class ConfirmChangePhoneNumberCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    ICodeManager codeManager,
    IRollbackManager rollbackManager,
    IMessageService messageService)
    : IRequestHandler<ConfirmChangePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IRollbackManager rollbackManager = rollbackManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(ConfirmChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var isTaken = await userManager.CheckPhoneNumberAsync(request.Request.NewPhoneNumber, cancellationToken);

        if (isTaken)
        {
            return Results.BadRequest("This phone number is already taken");
        }
        
        var codeResult = await codeManager.VerifyAsync(user, request.Request.Code, SenderType.Sms, 
            CodeType.New, CodeResource.PhoneNumber, cancellationToken);

        if (!codeResult.Succeeded)
        {
            return codeResult;
        }

        var rollback = await rollbackManager.CommitAsync(user, user.PhoneNumber, RollbackField.PhoneNumber, cancellationToken);

        if (rollback is null)
        {
            return Results.InternalServerError("Cannot change phone number, rollback was not created.");
        }
        
        var result = await userManager.ChangePhoneNumberAsync(user, request.Request.NewPhoneNumber, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }
        
        var message = new PhoneNumberChangedMessage()
        {
            Credentials = new()
            {
                { "PhoneNumber", rollback.Value }
            },
            Payload = new()
            {
                { "Code", rollback.Code },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Sms, message, cancellationToken);
        
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