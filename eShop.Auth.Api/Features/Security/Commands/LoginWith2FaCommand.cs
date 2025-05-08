using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;
using User = eShop.Domain.Types.User;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record LoginWith2FaCommand(LoginWith2FaRequest Request)
    : IRequest<Result>;

internal sealed class LoginWith2FaCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager) : IRequestHandler<LoginWith2FaCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(LoginWith2FaCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var result = await userManager.VerifyTwoFactorTokenAsync(user, "Email", request.Request.Code);

        if (!result)
        {
            return Results.BadRequest($"Invalid two-factor code {request.Request.Code}.");
        }

        var userDto = new User(user.Email!, user.UserName!, user.Id);
        var securityToken = await tokenManager.FindAsync(user, cancellationToken);

        if (securityToken is not null)
        {
            var token = await tokenManager.RefreshAsync(user, securityToken, cancellationToken);

            return Result.Success(new LoginResponse()
            {
                User = userDto,
                RefreshToken = token.RefreshToken,
                AccessToken = token.AccessToken,
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