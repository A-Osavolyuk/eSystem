using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record VerifyCurrentPhoneNumberCommand(VerifyCurrentPhoneNumberRequest Request) : IRequest<Result>;

public class VerifyCurrentPhoneNumberCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService,
    IdentityOptions identityOptions) : IRequestHandler<VerifyCurrentPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(VerifyCurrentPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        if (identityOptions.Account.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.NewPhoneNumber, cancellationToken);

            if (isTaken)
            {
                return Results.BadRequest("This phone number is already taken");
            }
        }
        
        var codeResult = await codeManager.VerifyAsync(user, request.Request.Code, 
            SenderType.Sms, CodeType.Current, CodeResource.PhoneNumber, cancellationToken);

        if (!codeResult.Succeeded)
        {
            return codeResult;
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.New, 
            CodeResource.PhoneNumber, cancellationToken);
        
        var message = new VerifyPhoneNumberMessage()
        {
            Credentials = new ()
            {
                { "PhoneNumber", request.Request.NewPhoneNumber },
            }, 
            Payload = new()
            {
                { "Code", code }
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Sms, message, cancellationToken);

        return Result.Success();
    }
}