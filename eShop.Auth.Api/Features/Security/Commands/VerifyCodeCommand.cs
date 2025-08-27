using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record VerifyCodeCommand(VerifyCodeRequest Request) : IRequest<Result>;

public class VerifyCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager) : IRequestHandler<VerifyCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var code = request.Request.Code;
        var sender = request.Request.Sender;
        var type = request.Request.Type;
        var resource = request.Request.Resource;
        
        var result = await codeManager.VerifyAsync(user, code, sender, type, resource, cancellationToken);
        return result;
    }
}