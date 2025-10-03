using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Verification.Commands;

public record VerifyCodeCommand(VerifyCodeRequest Request) : IRequest<Result>;

public class VerifyCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var code = request.Request.Code;
        var sender = request.Request.Sender;
        var type = request.Request.Type;
        var resource = request.Request.Resource;
        
        var codeResult = await codeManager.VerifyAsync(user, code, sender, type, resource, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var result = await verificationManager.CreateAsync(user, resource, type, cancellationToken);
        return result;
    }
}