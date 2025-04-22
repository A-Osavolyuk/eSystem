using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyCodeCommand(VerifyCodeRequest Request) : IRequest<Result>;

internal sealed class VerifyCodeCommandHandler(AppManager manager)
    : IRequestHandler<VerifyCodeCommand, Result>
{
    private readonly AppManager manager = manager;

    public async Task<Result> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var result = await manager.SecurityManager.VerifyCodeAsync(request.Request.Code, request.Request.Destination,
            request.Request.CodeType);

        if (!result.Succeeded)
        {
            return Results.InternalServerError(result.Errors.First().Description);
        }

        return Result.Success("Code was successfully verified");
    }
}