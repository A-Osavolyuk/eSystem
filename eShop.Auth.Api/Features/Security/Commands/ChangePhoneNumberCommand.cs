using eShop.Domain.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request)
    : IRequest<Result>;

internal sealed class RequestChangePhoneNumberCommandHandler(
    AppManager appManager,
    IMessageService messageService,
    IConfiguration configuration,
    ICodeManager codeManager) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

    public async Task<Result> Handle(ChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByPhoneNumberAsync(request.Request.CurrentPhoneNumber);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with phone number {request.Request.CurrentPhoneNumber}.");
        }

        var destinationSet = new DestinationSet()
        {
            Current = user.PhoneNumber!,
            Next = request.Request.NewPhoneNumber
        };
        
        var oldPhoneNumberCode = await codeManager.GenerateAsync(user, Verification.OldPhoneNumber);
        var newPhoneNumberCode = await codeManager.GenerateAsync(user, Verification.NewPhoneNumber);

        await messageService.SendMessageAsync("phone-number-change", new ChangePhoneNumberMessage()
        {
            Code = oldPhoneNumberCode,
            PhoneNumber = request.Request.NewPhoneNumber
        }, cancellationToken);

        await messageService.SendMessageAsync("phone-number-verification", new ChangePhoneNumberMessage()
        {
            Code = newPhoneNumberCode,
            PhoneNumber = request.Request.NewPhoneNumber
        }, cancellationToken);

        return Result.Success("We have sent sms messages to your phone numbers.");
    }
}