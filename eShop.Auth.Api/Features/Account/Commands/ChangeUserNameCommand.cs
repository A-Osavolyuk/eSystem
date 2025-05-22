using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record ChangeUserNameCommand(ChangeUserNameRequest Request)
    : IRequest<Result>;

internal sealed class ChangeUserNameCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager) : IRequestHandler<ChangeUserNameCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;

    public async Task<Result> Handle(ChangeUserNameCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var result = await userManager.SetUserNameAsync(user, request.Request.UserName, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }
        
        var tokens = await tokenManager.GenerateAsync(user, cancellationToken);

        return Result.Success(new ChangeUserNameResponse()
        {
            Message = "Your user name was successfully changed.",
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        });
    }
}