using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request) : IRequest<Result>;

public sealed class RequestChangePhoneNumberCommandHandler(
    IMessageService messageService,
    ICodeManager codeManager,
    IUserManager userManager) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(ChangePhoneNumberCommand request,
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

        var oldPhoneNumberCode = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.Current, 
            CodeResource.PhoneNumber, cancellationToken);
        var newPhoneNumberCode = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.New, 
            CodeResource.PhoneNumber, cancellationToken);

        var stepOneMessage = new ChangePhoneNumberSmsMessage()
        {
            Credentials = new ()
            {
                { "PhoneNumber", user.PhoneNumber },
            }, 
            Payload = new()
            {
                { "Code", oldPhoneNumberCode }
            },
        };
        
        await messageService.SendMessageAsync(SenderType.Sms, stepOneMessage, cancellationToken);
        
        var stepTwoMessage = new VerifyPhoneNumberSmsMessage()
        {
            Credentials = new ()
            {
                { "PhoneNumber", request.Request.NewPhoneNumber },
            }, 
            Payload = new()
            {
                { "Code", newPhoneNumberCode }
            }
        };

        return Result.Success();
    }
}