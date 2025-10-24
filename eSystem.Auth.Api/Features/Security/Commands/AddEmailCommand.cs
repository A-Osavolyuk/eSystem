using eSystem.Auth.Api.Interfaces;
using eSystem.Auth.Api.Security.Identity.Options;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record AddEmailCommand(AddEmailRequest Request) : IRequest<Result>;

public class AddEmailCommandHandler(
    IUserManager userManager,
    IOptions<AccountOptions> options) : IRequestHandler<AddEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly AccountOptions options = options.Value;

    public async Task<Result> Handle(AddEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (user.Emails.Count(x => x is { Type: EmailType.Secondary }) >= options.SecondaryEmailMaxCount)
            return Results.BadRequest("User already has maximum count of secondary emails.");

        if (options.RequireUniqueEmail)
        {
            var taken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
            if (taken) return Results.BadRequest("Email already taken.");
        }

        var email = request.Request.Email;
        var result = await userManager.AddEmailAsync(user, email, EmailType.Secondary, cancellationToken);
        
        return result;
    }
}