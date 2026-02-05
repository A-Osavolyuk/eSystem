using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authorization.OAuth.Consents;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Connect.Commands;

public record CheckConsentCommand(CheckConsentRequest Request) : IRequest<Result>;

public class CheckConsentCommandHandler(
    IConsentManager consentManager,
    IClientManager clientManager,
    IUserManager userManager) : IRequestHandler<CheckConsentCommand, Result>
{
    private readonly IConsentManager _consentManager = consentManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(CheckConsentCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null) return Results.NotFound("Client not found");

        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null)
        {
            return Results.Ok(new CheckConsentResponse
            {
                Granted = false,
                RemainingScopes = request.Request.Scopes
            });
        }

        if (!consent.HasScopes(request.Request.Scopes, out var remainingScopes))
        {
            return Results.Ok(new CheckConsentResponse
            {
                Granted = false,
                RemainingScopes = remainingScopes.ToList()
            });
        }
        
        return Results.Ok(new CheckConsentResponse { Granted = true });
    }
}