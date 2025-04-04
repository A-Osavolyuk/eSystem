namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record ChangeEmailCommand(ChangeEmailRequest Request) : IRequest<Result>;

internal sealed class RequestChangeEmailCommandHandler(
    AppManager appManager,
    IMessageService messageService,
    IConfiguration configuration) : IRequestHandler<ChangeEmailCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;
    private readonly IConfiguration configuration = configuration;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;

    public async Task<Result> Handle(ChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.CurrentEmail);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.CurrentEmail}"
            });
        }

        var destination = new DestinationSet()
        {
            Current = request.Request.CurrentEmail,
            Next = request.Request.NewEmail
        };
        var code = await appManager.SecurityManager.GenerateVerificationCodeSetAsync(destination,
            VerificationCodeType.ChangeEmail);

        await messageService.SendMessageAsync("email-change", new ChangeEmailMessage()
        {
            Code = code.Current,
            To = request.Request.CurrentEmail,
            Subject = "Email change (step one)",
            UserName = request.Request.CurrentEmail,
            NewEmail = request.Request.NewEmail,
        });

        await messageService.SendMessageAsync("email-verification", new EmailVerificationMessage()
        {
            Code = code.Next,
            UserName = request.Request.CurrentEmail,
            Subject = "Email change (step two)",
            To = request.Request.NewEmail,
        });

        return Result.Success("We have sent a letter with instructions to your current and new email addresses");
    }
}