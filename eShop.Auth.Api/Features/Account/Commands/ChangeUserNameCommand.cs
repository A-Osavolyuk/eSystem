using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record ChangeUserNameCommand(ChangeUserNameRequest Request)
    : IRequest<Result>;

internal sealed class ChangeUserNameCommandHandler(
    AuthDbContext context,
    UserManager<UserEntity> userManager,
    ITokenManager tokenManager) : IRequestHandler<ChangeUserNameCommand, Result>
{
    private readonly AuthDbContext context = context;
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;

    public async Task<Result> Handle(ChangeUserNameCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email);
        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var result = await userManager.SetUserNameAsync(user, request.Request.UserName);

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Internal server error",
                Details = $"Cannot change username of user with email {request.Request.Email} " +
                          $"due to error: {result.Errors.First().Description}."
            });
        }

        user = await userManager.FindByEmailAsync(request.Request.Email);

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