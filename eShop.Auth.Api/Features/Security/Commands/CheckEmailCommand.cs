using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record CheckEmailCommand(CheckEmailRequest Request) : IRequest<Result>;

public class CheckEmailCommandHandler(
    IUserManager userManager) : IRequestHandler<CheckEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(CheckEmailCommand request, CancellationToken cancellationToken)
    {
        CheckEmailResponse? response;

        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var isTaken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
        if (!isTaken)
        {
            response = new CheckEmailResponse() { IsTaken = false };
            return Result.Success(response);
        }

        var userEmail = user.Emails.FirstOrDefault(x => x.Email == request.Request.Email);
        if (userEmail is null)
        {
            response = new CheckEmailResponse() { IsTaken = true };
            return Result.Success(response);
        }

        if (userEmail.Type == EmailType.Primary)
        {
            var hasTwoFactor = user.TwoFactorProviders.Any(x => x is
                { Subscribed: true, TwoFactorProvider.Name: ProviderTypes.Email });

            var hasLinkedAccount = user.HasLinkedAccount();

            response = new CheckEmailResponse()
            {
                IsOwn = true,
                IsTaken = true,
                Type = userEmail.Type,
                HasTwoFactor = hasTwoFactor,
                HasLinkedAccount = hasLinkedAccount
            };
            
            return Result.Success(response);
        }

        response = new CheckEmailResponse()
        {
            IsOwn = true,
            IsTaken = true,
            Type = userEmail.Type,
        };

        return Result.Success(response);
    }
}