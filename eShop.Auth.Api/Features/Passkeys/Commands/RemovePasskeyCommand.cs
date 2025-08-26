using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record RemovePasskeyCommand(RemovePasskeyRequest Request) : IRequest<Result>;

public class RemovePasskeyCommandHandler(
    IPasskeyManager passkeyManager,
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService,
    IdentityOptions identityOptions) : IRequestHandler<RemovePasskeyCommand, Result>
{
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(RemovePasskeyCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var passkey = await passkeyManager.FindByIdAsync(request.Request.KeyId, cancellationToken);
        if (passkey is null) return Results.NotFound($"Cannot find passkey with ID {request.Request.KeyId}.");

        var code = await codeManager.GenerateAsync(user, SenderType.Email,
            CodeType.Remove, CodeResource.Passkey, cancellationToken);

        if (identityOptions.SignIn.RequireConfirmedEmail && user.HasEmail()
            && identityOptions.SignIn.RequireConfirmedPhoneNumber && user.HasPhoneNumber()
            && identityOptions.SignIn.AllowOAuthLogin && user.HasLinkedAccount() && user.HasPassword())
            return Results.BadRequest("You need to enable another authentication method first.");

        var message = new RemovePasskeyMessage()
        {
            Credentials = new()
            {
                { "To", user!.Email },
                { "Subject", "Device block" }
            },
            Payload = new()
            {
                { "Code", code },
                { "UserName", user.UserName },
                { "DisplayName", passkey.DisplayName }
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        return Result.Success();
    }
}