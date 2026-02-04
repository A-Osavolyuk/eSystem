using eSecurity.Core.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Connect.Queries;

public record GetClientInfoQuery(string ClientId) : IRequest<Result>;

public class GetClientInfoQueryHandler(IClientManager clientManager) : IRequestHandler<GetClientInfoQuery, Result>
{
    private readonly IClientManager _clientManager = clientManager;

    public async Task<Result> Handle(GetClientInfoQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientManager.FindByIdAsync(request.ClientId, cancellationToken);
        if (client is null) return Results.NotFound("Client not found");

        var response = new ClientInfo
        {
            ClientName = client.Name,
            ClientType = client.ClientType,
            LogoUri = client.LogoUri,
            ClientUri = client.ClientUri,
            RequiredScopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList(),
        };
        
        return Results.Ok(response);
    }
}