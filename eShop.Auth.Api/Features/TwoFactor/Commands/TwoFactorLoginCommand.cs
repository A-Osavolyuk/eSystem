using eShop.Domain.Common.Security;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

internal sealed record TwoFactorLoginCommand(TwoFactorLoginRequest Request)
    : IRequest<Result>;

internal sealed class LoginWith2FaCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    ILoginTokenManager loginTokenManager,
    IProviderManager providerManager) : IRequestHandler<TwoFactorLoginCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
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

        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        return Result.Success(new LoginResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Message = "Successfully logged in."
        });
    }
}