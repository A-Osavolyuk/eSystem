using eSecurity.Core.Common.Requests;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authorization.Consents;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Server.Features.Connect.Commands;

public record GrantConsentCommand(GrantConsentRequest Request) : IRequest<Result>;

public class GrantConsentCommandHandler(
    IUserManager userManager,
    IClientManager clientManager,
    IConsentManager consentManager,
    IOptions<OpenIdConfiguration> options) : IRequestHandler<GrantConsentCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly OpenIdConfiguration _options = options.Value;

    public async Task<Result> Handle(GrantConsentCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null)
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "user_id is invalid."
            });

        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null)
            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Invalid client."
            });

        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null)
        {
            consent = new ConsentEntity
            {
                UserId = user.Id,
                ClientId = client.Id,
            };

            var createResult = await _consentManager.CreateAsync(consent, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            foreach (var requestedScope in request.Request.Scopes)
            {
                if (!_options.ScopesSupported.Contains(requestedScope))
                {
                    return Results.BadRequest(new Error
                    {
                        Code = ErrorTypes.OAuth.InvalidScope,
                        Description = $"'{requestedScope}' scope is not supported."
                    });
                }

                var grantResult = await _consentManager.GrantAsync(consent, requestedScope, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }

            return Results.Ok();
        }

        if (!consent.HasScopes(request.Request.Scopes, out var remainingScopes))
        {
            foreach (var requestedScope in remainingScopes)
            {
                if (!_options.ScopesSupported.Contains(requestedScope))
                {
                    return Results.BadRequest(new Error
                    {
                        Code = ErrorTypes.OAuth.InvalidScope,
                        Description = $"'{requestedScope}' scope is not supported."
                    });
                }

                var grantResult = await _consentManager.GrantAsync(consent, requestedScope, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }
        }
        
        return Results.Ok();
    }
}