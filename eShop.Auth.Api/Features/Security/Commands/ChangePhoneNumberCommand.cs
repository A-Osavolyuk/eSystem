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

        var oldPhoneNumberCode = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.Current, cancellationToken);
        var newPhoneNumberCode = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.New, cancellationToken);

        var credentials = new SmsCredentials() { PhoneNumber = request.Request.NewPhoneNumber };
        
        await messageService.SendMessageAsync(SenderType.Email, "phone-number/change",
            new { Code = oldPhoneNumberCode, }, credentials, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Email, "phone-number/verify",
            new { Code = newPhoneNumberCode, }, credentials, cancellationToken);

        return Result.Success();
    }
}