using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AddPasswordCommand(AddPasswordRequest Request) : IRequest<Result>;

public class AddPasswordCommandHandler(
    IUserManager userManager,
    ILoginMethodManager loginMethodManager) : IRequestHandler<AddPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILoginMethodManager loginMethodManager = loginMethodManager;

    public async Task<Result> Handle(AddPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
        if (user.HasPassword()) return Results.BadRequest("User already has a password.");

        if (!user.HasLoginMethod(LoginType.Password))
        {
            var methodResult = await loginMethodManager.CreateAsync(user, LoginType.Password, cancellationToken);
            if (!methodResult.Succeeded) return methodResult;
        }
        
        var result = await userManager.AddPasswordAsync(user, request.Request.Password, cancellationToken);
        return result;
    }
}