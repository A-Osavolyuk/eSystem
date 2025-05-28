using eShop.Domain.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request)
    : IRequest<Result>;

internal sealed class RequestChangePhoneNumberCommandHandler(
    IMessageService messageService,
    IConfiguration configuration,
    ICodeManager codeManager,
    IUserManager userManager) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IUserManager userManager = userManager;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

    public async Task<Result> Handle(ChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByPhoneNumberAsync(request.Request.CurrentPhoneNumber, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with phone number {request.Request.CurrentPhoneNumber}.");
        }
        
        var oldPhoneNumberCode = await codeManager.GenerateAsync(user, CodeType.Current, cancellationToken);
        var newPhoneNumberCode = await codeManager.GenerateAsync(user, CodeType.New, cancellationToken);

        await messageService.SendMessageAsync("sms:change-phone-number", new ChangePhoneNumberMessage()
        {
            Code = oldPhoneNumberCode,
            PhoneNumber = request.Request.NewPhoneNumber
        }, cancellationToken);

        await messageService.SendMessageAsync("sms:verify-phone-number", new ChangePhoneNumberMessage()
        {
            Code = newPhoneNumberCode,
            PhoneNumber = request.Request.NewPhoneNumber
        }, cancellationToken);

        return Result.Success("We have sent sms messages to your phone numbers.");
    }
}