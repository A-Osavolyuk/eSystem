using eShop.Domain.Common.Security;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;
using User = eShop.Domain.Types.User;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

internal sealed record TwoFactorLoginCommand(TwoFactorLoginRequest Request)
    : IRequest<Result>;

internal sealed class LoginWith2FaCommandHandler(
    ISecurityTokenManager securityTokenManager,
    IUserManager userManager,
    ILoginTokenManager loginTokenManager,
    IProviderManager providerManager) : IRequestHandler<TwoFactorLoginCommand, Result>
{
    private readonly ISecurityTokenManager securityTokenManager = securityTokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly ILoginTokenManager loginTokenManager = loginTokenManager;
    private readonly IProviderManager providerManager = providerManager;

    public async Task<Result> Handle(TwoFactorLoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var provider = await providerManager.FindAsync(request.Request.Provider, cancellationToken);

        if (provider is null)
        {
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        }

        var token = request.Request.Token;
        var result = await loginTokenManager.VerifyAsync(user, provider, token, cancellationToken);

        if (!result.Succeeded)
        {
            return Results.BadRequest($"Invalid two-factor token {request.Request.Token}.");
        }

        var userDto = new User(user.Email!, user.UserName!, user.Id);
        var securityToken = await securityTokenManager.FindAsync(user, cancellationToken);

        if (securityToken is not null)
        {
            var accessToken = await securityTokenManager.RefreshAsync(user, securityToken, cancellationToken);

            return Result.Success(new LoginResponse()
            {
                User = userDto,
                RefreshToken = accessToken.RefreshToken,
                AccessToken = accessToken.AccessToken,
                Message = "Successfully logged in.",
                HasTwoFactorAuthentication = false
            });
        }

        var tokens = await securityTokenManager.GenerateAsync(user, cancellationToken);

        return Result.Success(new LoginResponse()
        {
            User = userDto,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            Message = "Successfully logged in."
        });
    }
}