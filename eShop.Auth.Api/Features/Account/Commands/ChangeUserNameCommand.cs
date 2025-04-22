using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record ChangeUserNameCommand(ChangeUserNameRequest Request)
    : IRequest<Result>;

internal sealed class ChangeUserNameCommandHandler(
    ITokenHandler tokenHandler,
    AppManager appManager,
    AuthDbContext context) : IRequestHandler<ChangeUserNameCommand, Result>
{
    private readonly ITokenHandler tokenHandler = tokenHandler;
    private readonly AppManager appManager = appManager;
    private readonly AuthDbContext context = context;

    public async Task<Result> Handle(ChangeUserNameCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);
        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var result = await appManager.UserManager.SetUserNameAsync(user, request.Request.UserName);

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

        user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with email {request.Request.Email}."
            });
        }

        var roles = await appManager.UserManager.GetRolesAsync(user);
        var permissions = await appManager.PermissionManager.GetUserPermissionsAsync(user, cancellationToken);
        var tokens = await tokenHandler.GenerateTokenAsync(user!, roles.ToList(), permissions);

        return Result.Success(new ChangeUserNameResponse()
        {
            Message = "Your user name was successfully changed.",
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        });
    }
}