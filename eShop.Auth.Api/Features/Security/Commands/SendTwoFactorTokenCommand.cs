using eShop.Domain.Common.Security;
using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record SendTwoFactorTokenCommand(SendTwoFactorTokenRequest Request) : IRequest<Result>;

public class SendTwoFactorTokenCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IMessageService messageService) : IRequestHandler<SendTwoFactorTokenCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(SendTwoFactorTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Request.Email}.");
        }

        var provider = await twoFactorManager.GetProviderAsync(request.Request.Provider, cancellationToken);

        if (provider is null)
        {
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        }
        
        var token = await twoFactorManager.GenerateTokenAsync(user, provider, cancellationToken);
        
        return Result.Success();
    }
}