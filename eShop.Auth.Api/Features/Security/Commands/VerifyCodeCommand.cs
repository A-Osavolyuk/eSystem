namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyCodeCommand(VerifyCodeRequest Request) : IRequest<Result<VerifyCodeResponse>>;

internal sealed class VerifyCodeCommandHandler(AppManager manager)
    : IRequestHandler<VerifyCodeCommand, Result<VerifyCodeResponse>>
{
    private readonly AppManager manager = manager;

    public async Task<Result<VerifyCodeResponse>> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var result = await manager.SecurityManager.VerifyCodeAsync(request.Request.Code, request.Request.Destination,
            request.Request.CodeType);

        if (!result.Succeeded)
        {
            return new(new NotFoundException(result.Errors.First().Description));
        }

        return new(new VerifyCodeResponse() { Message = "Code was successfully verified" });
    }
}