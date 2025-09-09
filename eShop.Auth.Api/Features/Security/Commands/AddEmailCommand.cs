using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AddEmailCommand(AddEmailRequest Request) : IRequest<Result>;

public class AddEmailCommandHandler(
    IUserManager userManager,
    IdentityOptions identityOptions) : IRequestHandler<AddEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(AddEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (user.Emails.Count(x => x.Type is EmailType.Primary) 
            >= identityOptions.Account.PrimaryEmailMaxCount && request.Request.Type is EmailType.Primary) 
            return Results.BadRequest("User already has a primary email.");

        if (user.Emails.Count(x => x.Type is EmailType.Secondary) 
            >= identityOptions.Account.RecoveryEmailMaxCount && request.Request.Type is EmailType.Recovery) 
            return Results.BadRequest("User already has a recovery email.");

        if (user.Emails.Count(x => x is { Type: EmailType.Secondary })
            >= identityOptions.Account.SecondaryEmailMaxCount && request.Request.Type is EmailType.Secondary)
            return Results.BadRequest("User already has maximum count of secondary emails.");

        if (identityOptions.Account.RequireUniqueEmail)
        {
            var taken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
            if (taken) return Results.BadRequest("Email already taken.");
        }

        var result = await userManager.AddEmailAsync(user, request.Request.Email,
            request.Request.Type, cancellationToken);

        return result;
    }
}