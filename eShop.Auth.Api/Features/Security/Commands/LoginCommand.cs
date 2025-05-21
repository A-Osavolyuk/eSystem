using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record LoginCommand(LoginRequest Request) : IRequest<Result>;

internal sealed class LoginCommandHandler(
    ISecurityTokenManager securityTokenManager,
    IUserManager userManager,
    IMessageService messageService,
    ITwoFactorManager twoFactorManager) : IRequestHandler<LoginCommand, Result>
{
    private readonly ISecurityTokenManager securityTokenManager = securityTokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        if (!user.EmailConfirmed)
        {
            return Results.BadRequest("The email address is not confirmed.");
        }

        var isValidPassword = await userManager.CheckPasswordAsync(user, request.Request.Password, cancellationToken);

        if (!isValidPassword)
        {
            return Results.BadRequest("The password is not valid.");
        }
        
        if (user.TwoFactorEnabled)
        {
            return Result.Success(new LoginResponse()
            {
                HasTwoFactorAuthentication = true
            });
        }
        
        var tokens = await securityTokenManager.GenerateAsync(user, cancellationToken);

        return Result.Success(new LoginResponse()
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            Message = "Successfully logged in.",
            HasTwoFactorAuthentication = false
        });
    }
}