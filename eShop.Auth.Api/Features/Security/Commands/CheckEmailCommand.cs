using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record CheckEmailCommand(CheckEmailRequest Request) : IRequest<Result>;

public class CheckEmailCommandHandler(
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        if (identityOptions.Account.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
            if (isTaken) return Results.BadRequest("This email address is already taken");
        }
        
        return Result.Success();
    }
}