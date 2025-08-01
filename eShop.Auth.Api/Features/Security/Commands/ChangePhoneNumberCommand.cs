using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request) : IRequest<Result>;

public sealed class RequestChangePhoneNumberCommandHandler(
    IMessageService messageService,
    ICodeManager codeManager,
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(ChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
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

        var code = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.Current, 
            CodeResource.PhoneNumber, cancellationToken);

        var message = new ChangePhoneNumberMessage()
        {
            Credentials = new ()
            {
                { "PhoneNumber", user.PhoneNumber },
            }, 
            Payload = new()
            {
                { "Code", code }
            },
        };
        
        await messageService.SendMessageAsync(SenderType.Sms, message, cancellationToken);

        return Result.Success();
    }
}