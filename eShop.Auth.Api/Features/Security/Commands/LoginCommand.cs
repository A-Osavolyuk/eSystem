using eShop.Domain.Common.API;
using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record LoginCommand(LoginRequest Request) : IRequest<Result>;

internal sealed class LoginCommandHandler(
    AppManager appManager,
    IMessageService messageService,
    ITokenHandler tokenHandler) : IRequestHandler<LoginCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;
    private readonly ITokenHandler tokenHandler = tokenHandler;

    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        if (!user.EmailConfirmed)
        {
            return Results.BadRequest("The email address is not confirmed.");
        }

        var isValidPassword = await appManager.UserManager.CheckPasswordAsync(user, request.Request.Password);

        if (!isValidPassword)
        {
            return Results.BadRequest("The password is not valid.");
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

        if (user.TwoFactorEnabled)
        {
            var loginCode = await appManager.UserManager.GenerateTwoFactorTokenAsync(user, "Email");

            await messageService.SendMessageAsync("2fa-code", new TwoFactorAuthenticationCodeMessage()
            {
                To = user.Email!,
                Subject = "Login with 2FA code",
                UserName = user.UserName!,
                Code = loginCode
            }, cancellationToken);

            return Result.Success(new LoginResponse()
            {
                User = userDto,
                Message = "We have sent an email with 2FA code at your email address.",
                HasTwoFactorAuthentication = true
            });
        }
        
        var tokens = await tokenHandler.GenerateTokenAsync(user);

        return Result.Success(new LoginResponse()
        {
            User = userDto,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            Message = "Successfully logged in.",
            HasTwoFactorAuthentication = false
        });
    }
}