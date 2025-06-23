using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record VerifyCodeCommand(VerifyCodeRequest Request) : IRequest<Result>;

public sealed class VerifyCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager)
    : IRequestHandler<VerifyCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound("User not found.");
        }
        
        var result = await codeManager.VerifyAsync(user, request.Request.Code, request.Request.Type, cancellationToken);

        return result;
    }
}