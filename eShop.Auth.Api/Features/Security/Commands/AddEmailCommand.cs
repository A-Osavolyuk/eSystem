using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AddEmailCommand(AddEmailRequest Request) : IRequest<Result>;

public class AddEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<AddEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(AddEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var taken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
        if (taken) return Results.BadRequest("Email already taken.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            CodeResource.Email, CodeType.Verify, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var result = await userManager.AddEmailAsync(user, request.Request.Email, cancellationToken);
        return result;
    }
}