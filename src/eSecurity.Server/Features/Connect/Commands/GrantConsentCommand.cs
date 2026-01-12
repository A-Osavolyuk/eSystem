using eSecurity.Core.Common.Requests;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authorization.Consents;
using eSecurity.Server.Security.Authorization.Scopes;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Connect.Commands;

public record GrantConsentCommand(GrantConsentRequest Request) : IRequest<Result>;

public class GrantConsentCommandHandler(
    IUserManager userManager,
    IClientManager clientManager,
    IScopeManager scopeManager,
    IConsentManager consentManager) : IRequestHandler<GrantConsentCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IScopeManager _scopeManager = scopeManager;
    private readonly IConsentManager _consentManager = consentManager;

    public async Task<Result> Handle(GrantConsentCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null)
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "user_id is invalid."
            });

        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
            return Results.Unauthorized(new Error()
            {
                Code = Errors.OAuth.InvalidClient,
                Description = "Invalid client."
            });

        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null)
        {
            consent = new ConsentEntity()
            {
                UserId = user.Id,
                ClientId = client.Id,
            };

            var createResult = await _consentManager.CreateAsync(consent, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            foreach (var requestedScope in request.Request.Scopes)
            {
                var scope = await _scopeManager.FindByNameAsync(requestedScope, cancellationToken);
                if (scope is null)
                {
                    return Results.BadRequest(new Error()
                    {
                        Code = Errors.OAuth.InvalidScope,
                        Description = $"'{requestedScope}' scope is not supported."
                    });
                }

                var grantResult = await _consentManager.GrantAsync(consent, scope, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }

            return Results.Ok();
        }

        if (!consent.HasScopes(request.Request.Scopes, out var remainingScopes))
        {
            foreach (var requestedScope in remainingScopes)
            {
                var scope = await _scopeManager.FindByNameAsync(requestedScope, cancellationToken);
                if (scope is null)
                {
                    return Results.BadRequest(new Error()
                    {
                        Code = Errors.OAuth.InvalidScope,
                        Description = $"'{requestedScope}' scope is not supported."
                    });
                }

                var grantResult = await _consentManager.GrantAsync(consent, scope, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }
        }
        
        return Results.Ok();
    }
}