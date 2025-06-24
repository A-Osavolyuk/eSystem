using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record VerifyPhoneNumberCommand(VerifyPhoneNumberRequest Request) : IRequest<Result>;

public sealed class VerifyPhoneNumberCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager) : IRequestHandler<VerifyPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID ${request.Request.Id}");
        }
        
        var result = await codeManager.VerifyAsync(user, request.Request.Code, SenderType.Sms, 
            CodeType.Verify, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        var confirmationResult = await userManager.ConfirmPhoneNumberAsync(user, cancellationToken);

        return confirmationResult;
    }
}