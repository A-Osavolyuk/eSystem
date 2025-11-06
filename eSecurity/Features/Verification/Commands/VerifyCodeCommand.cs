using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Features.Verification.Commands;

public record VerifyCodeCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public required string Code { get; set; }
    public PurposeType Purpose  { get; set; }
    public ActionType Action  { get; set; }
    public SenderType Sender { get; set; }
}

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
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var code = request.Code;
        var sender = request.Sender;
        var action = request.Action;
        var purpose = request.Purpose;
        
        var codeResult = await codeManager.VerifyAsync(user, code, sender, action, purpose, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var result = await verificationManager.CreateAsync(user, purpose, action, cancellationToken);
        return result;
    }
}