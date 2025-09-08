using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AddEmailCommand(AddEmailRequest Request) : IRequest<Result>;

public class AddEmailCommandHandler(
    IUserManager userManager) : IRequestHandler<AddEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(AddEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (user.HasEmail()) return Results.BadRequest("User already has an email address.");

        var taken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
        if (taken) return Results.BadRequest("Email already taken.");
        
        if(request.Request is { IsRecovery: true, IsPrimary: true }) 
            return Results.BadRequest("Same email cannot be primary and recovery at the same time.");

        var result = await userManager.AddEmailAsync(user, request.Request.Email,
            request.Request.IsPrimary, request.Request.IsRecovery, cancellationToken: cancellationToken);

        return result;
    }
}