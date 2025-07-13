using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ConfirmResetEmailCommand(ConfirmResetEmailRequest Request) : IRequest<Result>;

public class ConfirmResetEmailHandler(
    IUserManager userManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmResetEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmResetEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }

        var code = request.Request.Code;
        
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Email, 
            CodeType.Reset, CodeResource.Email, cancellationToken);

        if (!codeResult.Succeeded)
        {
            return codeResult;
        }
        
        var newEmail = request.Request.NewEmail;
        
        var result = await userManager.ResetEmailAsync(user, newEmail, cancellationToken);
        
        return result;
    }
}