using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

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
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var response = new CheckEmailResponse()
        {
            HasLinkedAccount = user.HasLinkedAccount(),
        };
        
        if (identityOptions.Account.RequireUniqueEmail)
        {
            response.IsTaken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
            return Result.Success(response);
        }

        response.IsTaken = false;
        return Result.Success(response);
    }
}