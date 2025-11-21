using eSecurity.Core.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Authentication.Oidc.Constants;

namespace eSecurity.Server.Features.Connect.Queries;

public record GetClientInfoQuery(string ClientId) : IRequest<Result>;

public class GetClientInfoQueryHandler(IClientManager clientManager) : IRequestHandler<GetClientInfoQuery, Result>
{
    private readonly IClientManager _clientManager = clientManager;

    public async Task<Result> Handle(GetClientInfoQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientManager.FindByIdAsync(request.ClientId, cancellationToken);
        if (client is null) return Results.NotFound("Client not found");

        var clientType = client.Type switch
        {
            ClientType.Public => ClientTypes.Public,
            ClientType.Confidential => ClientTypes.Confidential,
            _ => throw new NotSupportedException($"Client type {client.Type} not supported")
        };

        var response = new ClientInfo()
        {
            ClientName = client.Name,
            ClientType = clientType,
            LogoUri = client.LogoUri,
            ClientUri = client.ClientUri,
            RequiredScopes = client.AllowedScopes.Select(x => x.Scope.Name).ToList(),
        };
        
        return Results.Ok(response);
    }
}