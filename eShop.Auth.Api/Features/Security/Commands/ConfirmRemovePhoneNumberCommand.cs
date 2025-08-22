using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ConfirmRemovePhoneNumberCommand(ConfirmRemovePhoneNumberRequest Request) : IRequest<Result>;

public class ConfirmRemovePhoneNumberCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmRemovePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmRemovePhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var code = request.Request.Code;
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Sms, 
            CodeType.Remove, CodeResource.PhoneNumber, cancellationToken);

        if (!codeResult.Succeeded) return codeResult;
        
        var result = await userManager.RemovePhoneNumberAsync(user, cancellationToken);
        return result;
    }
}