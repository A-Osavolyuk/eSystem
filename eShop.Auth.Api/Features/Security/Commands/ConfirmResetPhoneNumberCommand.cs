using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ConfirmResetPhoneNumberCommand(ConfirmResetPhoneNumberRequest Request) : IRequest<Result>;

public class ConfirmResetPhoneNumberCommandHandler(
    ICodeManager codeManager,
    IUserManager userManager) : IRequestHandler<ConfirmResetPhoneNumberCommand, Result>
{
    private readonly ICodeManager codeManager = codeManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(ConfirmResetPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }

        var code = request.Request.Code;
        
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Sms, 
            CodeType.Reset, CodeResource.PhoneNumber, cancellationToken);

        if (!codeResult.Succeeded)
        {
            return codeResult;
        }
        
        var newPhoneNumber = request.Request.NewPhoneNumber;
        
        var result = await userManager.ResetPhoneNumberAsync(user, newPhoneNumber, cancellationToken);
        
        return result;
    }
}