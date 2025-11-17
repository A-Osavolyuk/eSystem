using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authentication.Oidc.Client;

namespace eSecurity.Server.Features.Connect.Queries;

public sealed record GetClientScopesQuery(string ClientId) : IRequest<Result>;

public class GetClientScopesQueryHandler(
    IClientManager clientManager) : IRequestHandler<GetClientScopesQuery, Result>
{
    private readonly IClientManager _clientManager = clientManager;

    public async Task<Result> Handle(GetClientScopesQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientManager.FindByIdAsync(request.ClientId, cancellationToken);
        if (client is null) return Results.NotFound("Client was not found.");

        var response = client.AllowedScopes
            .Select(allowedScope => allowedScope.Scope)
            .Select(scope => new ScopeDto()
            {
                Name = scope.Name,
                Description = scope.Description,
            }).ToList();
        
        return Results.Ok(response);
    }
}