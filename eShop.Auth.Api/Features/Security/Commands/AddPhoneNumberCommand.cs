using eShop.Domain.Abstraction.Messaging.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AddPhoneNumberCommand(AddPhoneNumberRequest Request) : IRequest<Result>;

public class AddPhoneNumberCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<AddPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(AddPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.Id}");
        }

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            return Results.BadRequest("User already has a phone number");
        }
        
        var result = await userManager.AddPhoneNumberAsync(user, request.Request.PhoneNumber, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.Verify, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Sms, "phone-number-verify", new { Code = code },
            new SmsCredentials() { PhoneNumber = request.Request.PhoneNumber }, cancellationToken);

        return Result.Success();
    }
}