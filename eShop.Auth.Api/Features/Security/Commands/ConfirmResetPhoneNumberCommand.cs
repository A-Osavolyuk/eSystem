using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ConfirmResetPhoneNumberCommand(ConfirmResetPhoneNumberRequest Request) : IRequest<Result>;

public class ConfirmResetPhoneNumberCommandHandler(
    ICodeManager codeManager,
    IUserManager userManager,
    IRollbackManager rollbackManager,
    IMessageService messageService) : IRequestHandler<ConfirmResetPhoneNumberCommand, Result>
{
    private readonly ICodeManager codeManager = codeManager;
    private readonly IUserManager userManager = userManager;
    private readonly IRollbackManager rollbackManager = rollbackManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(ConfirmResetPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }
        
        var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.NewPhoneNumber, cancellationToken);

        if (isTaken)
        {
            return Results.BadRequest("This phone number is already taken");
        }

        var code = request.Request.Code;
        
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Sms, 
            CodeType.Reset, CodeResource.PhoneNumber, cancellationToken);

        if (!codeResult.Succeeded)
        {
            return codeResult;
        }
        
        var newPhoneNumber = request.Request.NewPhoneNumber;
        
        var rollback = await rollbackManager.CommitAsync(user, user.PhoneNumber, RollbackField.PhoneNumber, cancellationToken);

        if (rollback is null)
        {
            return Results.InternalServerError("Cannot reset phone number, rollback was not created");
        }
        
        var result = await userManager.ResetPhoneNumberAsync(user, newPhoneNumber, cancellationToken);
        
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
        
        return result;
    }
}