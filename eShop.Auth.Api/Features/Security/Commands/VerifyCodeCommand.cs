using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record VerifyCodeCommand(VerifyCodeRequest Request) : IRequest<Result>;

public class VerifyCodeCommandHandler(
    IUserManager userManager,
    IVerificationCodeManager verificationCodeManager,
    IRecoverManager recoverManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationCodeManager verificationCodeManager = verificationCodeManager;
    private readonly IRecoverManager recoverManager = recoverManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var code = request.Request.Code;
        var sender = request.Request.Sender;
        var type = request.Request.Type;
        var resource = request.Request.Resource;

        var codeResult = await verificationCodeManager.VerifyAsync(user, code, sender, type, resource, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var recoverCodeResult = await recoverManager.VerifyAsync(user, code, cancellationToken);
            if (!recoverCodeResult.Succeeded) return codeResult;
        }

        var result = await verificationManager.CreateAsync(user, resource, type, cancellationToken);
        return result;
    }
}