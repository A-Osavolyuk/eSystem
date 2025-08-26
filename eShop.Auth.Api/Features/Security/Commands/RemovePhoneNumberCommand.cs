using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Common.Security;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RemovePhoneNumberCommand(RemovePhoneNumberRequest Request) :  IRequest<Result>;

public class RemovePhoneNumberCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<RemovePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(RemovePhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (!user.HasPhoneNumber()) return Results.BadRequest(
            "Cannot remove phone number. Phone number is not provided.");

        if (user.Providers.Any(x => x.Provider.Name == ProviderTypes.Sms && x.Subscribed))
            return Results.BadRequest("Cannot remove phone number. First disable 2FA with SMS.");

        var code = await codeManager.GenerateAsync(user, SenderType.Sms, 
            CodeType.Remove, CodeResource.PhoneNumber, cancellationToken);
        
        var message = new RemovePhoneNumberMessage()
        {
            Credentials = new Dictionary<string, string>()
            {
                { "PhoneNumber", user.PhoneNumber! },
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