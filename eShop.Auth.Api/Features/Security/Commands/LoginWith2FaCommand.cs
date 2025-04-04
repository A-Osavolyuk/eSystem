namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record LoginWith2FaCommand(LoginWith2FaRequest With2FaRequest)
    : IRequest<Result>;

internal sealed class LoginWith2FaCommandHandler(
    AppManager appManager,
    ITokenHandler tokenHandler) : IRequestHandler<LoginWith2FaCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly ITokenHandler tokenHandler = tokenHandler;

    public async Task<Result> Handle(LoginWith2FaCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.With2FaRequest.Email);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found.",
                Details = $"Cannot find user with email {request.With2FaRequest.Email}."
            });
        }

        var result =
            await appManager.UserManager.VerifyTwoFactorTokenAsync(user, "Email", request.With2FaRequest.Code);

        if (!result)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.BadRequest,
                Message = "Bad request.",
                Details = $"Invalid two-factor code {request.With2FaRequest.Code}."
            });
        }

        var userDto = new User(user.Email!, user.UserName!, user.Id);
        var securityToken = await appManager.SecurityManager.FindTokenAsync(user);

        if (securityToken is not null)
        {
            var token = await tokenHandler.RefreshTokenAsync(user, securityToken.Token);

            return Result.Success(new LoginResponse()
            {
                User = userDto,
                RefreshToken = token,
                Message = "Successfully logged in.",
                HasTwoFactorAuthentication = false
            });
        }
        else
        {
            var roles = (await appManager.UserManager.GetRolesAsync(user)).ToList();
            var permissions = await appManager.PermissionManager.GetUserPermissionsAsync(user);
            var tokens = await tokenHandler.GenerateTokenAsync(user, roles, permissions);

            return Result.Success(new LoginResponse()
            {
                User = userDto,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                Message = "Successfully logged in."
            });
        }
    }
}