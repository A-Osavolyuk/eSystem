using eShop.Domain.Messages.Sms;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request)
    : IRequest<Result>;

internal sealed class RequestChangePhoneNumberCommandHandler(
    AppManager appManager,
    IMessageService messageService,
    IConfiguration configuration) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

    public async Task<Result> Handle(ChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByPhoneNumberAsync(request.Request.CurrentPhoneNumber);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with phone number {request.Request.CurrentPhoneNumber}."
            });
        }

        var destinationSet = new DestinationSet()
        {
            Current = user.PhoneNumber!,
            Next = request.Request.NewPhoneNumber
        };
        var code = await appManager.SecurityManager.GenerateVerificationCodeSetAsync(destinationSet,
            VerificationCodeType.ChangePhoneNumber);

        await messageService.SendMessageAsync("phone-number-change", new ChangePhoneNumberMessage()
        {
            Code = code.Current,
            PhoneNumber = request.Request.NewPhoneNumber
        });

        await messageService.SendMessageAsync("phone-number-verification", new ChangePhoneNumberMessage()
        {
            Code = code.Next,
            PhoneNumber = request.Request.NewPhoneNumber
        });

        return Result.Success("We have sent sms messages to your phone numbers.");
    }
}