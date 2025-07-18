using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ResetPhoneNumberCommand(ResetPhoneNumberRequest Request) : IRequest<Result>;

public class ResetPhoneNumberCommandHandler(
    ICodeManager codeManager,
    IUserManager userManager,
    IMessageService messageService) : IRequestHandler<ResetPhoneNumberCommand, Result>
{
    private readonly ICodeManager codeManager = codeManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(ResetPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }
        
        var isTaken = await userManager.CheckPhoneNumberAsync(request.Request.NewPhoneNumber, cancellationToken);

        if (isTaken)
        {
            return Results.BadRequest("This phone number is already taken");
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.Reset, 
            CodeResource.PhoneNumber, cancellationToken);
        
        var message = new ResetPhoneNumberMessage()
        {
            Payload = new()
            {
                { "Code", code },
            },
            Credentials = new Dictionary<string, string>()
            {
                { "PhoneNumber", request.Request.NewPhoneNumber },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Sms,  message, cancellationToken);

        return Result.Success();
    }
}