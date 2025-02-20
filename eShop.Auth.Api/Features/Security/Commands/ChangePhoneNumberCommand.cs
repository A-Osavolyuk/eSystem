namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request)
    : IRequest<Result<ChangePhoneNumberResponse>>;

internal sealed class RequestChangePhoneNumberCommandHandler(
    AppManager appManager,
    IMessageService messageService,
    IConfiguration configuration) : IRequestHandler<ChangePhoneNumberCommand, Result<ChangePhoneNumberResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;
    private readonly IConfiguration configuration = configuration;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

    public async Task<Result<ChangePhoneNumberResponse>> Handle(ChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByPhoneNumberAsync(request.Request.CurrentPhoneNumber);

        if (user is null)
        {
            return new(new NotFoundException(
                $"Cannot find user with phone number {request.Request.CurrentPhoneNumber}."));
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

        return new(new ChangePhoneNumberResponse()
        {
            Message = "We have sent sms messages to your phone numbers."
        });
    }
}