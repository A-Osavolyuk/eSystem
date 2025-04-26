using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyCodeCommand(VerifyCodeRequest Request) : IRequest<Result>;

internal sealed class VerifyCodeCommandHandler(
    AppManager manager,
    ICodeManager codeManager)
    : IRequestHandler<VerifyCodeCommand, Result>
{
    private readonly AppManager manager = manager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await manager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Results.NotFound("User not found.");
        }
        
        var result = await codeManager.VerifyAsync(user, request.Request.Code, request.Request.CodeType);

        if (!result.Succeeded)
        {
            return result;
        }

        return Result.Success("Code was successfully verified");
    }
}