using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ResetPhoneNumberCommand(ResetPhoneNumberRequest Request) : IRequest<Result>;

public class ResetPhoneNumberCommandHandler(
    ICodeManager codeManager,
    IUserManager userManager,
    IMessageService messageService) : IRequestHandler<ResetPhoneNumberCommand, Result>
{
    private readonly ICodeManager codeManager = codeManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(ResetPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.Id}");
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Sms, CodeType.Reset, cancellationToken);

        await messageService.SendMessageAsync(SenderType.Email, "phone-number-reset",
            new
            {
                Code = code
            },
            new EmailCredentials()
            {
                To = request.Request.NewPhoneNumber,
                UserName = user.UserName,
                Subject = "Email reset"
            }, cancellationToken);

        return Result.Success();
    }
}