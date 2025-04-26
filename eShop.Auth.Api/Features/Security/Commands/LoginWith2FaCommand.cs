using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;
using User = eShop.Domain.Types.User;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record LoginWith2FaCommand(LoginWith2FaRequest With2FaRequest)
    : IRequest<Result>;

internal sealed class LoginWith2FaCommandHandler(
    ITokenManager tokenManager,
    UserManager<UserEntity> userManager) : IRequestHandler<LoginWith2FaCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly UserManager<UserEntity> userManager = userManager;

    public async Task<Result> Handle(LoginWith2FaCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.With2FaRequest.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.With2FaRequest.Email}.");
        }

        var result =
            await userManager.VerifyTwoFactorTokenAsync(user, "Email", request.With2FaRequest.Code);

        if (!result)
        {
            return Results.BadRequest($"Invalid two-factor code {request.With2FaRequest.Code}.");
        }

        var userDto = new User(user.Email!, user.UserName!, user.Id);
        var securityToken = await tokenManager.FindAsync(user, cancellationToken);

        if (securityToken is not null)
        {
            var token = await tokenManager.RefreshAsync(user, securityToken.Token, cancellationToken);

            return Result.Success(new LoginResponse()
            {
                User = userDto,
                RefreshToken = token,
                Message = "Successfully logged in.",
                HasTwoFactorAuthentication = false
            });
        }

        var tokens = await tokenManager.GenerateAsync(user, cancellationToken);

        return Result.Success(new LoginResponse()
        {
            User = userDto,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            Message = "Successfully logged in."
        });
    }
}