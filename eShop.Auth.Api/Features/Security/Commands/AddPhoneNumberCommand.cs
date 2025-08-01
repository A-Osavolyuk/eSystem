using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AddPhoneNumberCommand(AddPhoneNumberRequest Request) : IRequest<Result>;

public class AddPhoneNumberCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService,
    IdentityOptions identityOptions) : IRequestHandler<AddPhoneNumberCommand, Result>
{
    public IdentityOptions IdentityOptions { get; } = identityOptions;
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(AddPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
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

        if (identityOptions.Account.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.PhoneNumber, cancellationToken);

            if (isTaken)
            {
                return Results.BadRequest("This phone number is already taken");
            }
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.Verify, 
            CodeResource.PhoneNumber, cancellationToken);
        
        var message = new VerifyPhoneNumberMessage()
        {
            Credentials = new Dictionary<string, string>()
            {
                { "PhoneNumber", request.Request.PhoneNumber },
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