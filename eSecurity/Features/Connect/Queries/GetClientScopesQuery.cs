using eSecurity.Common.DTOs;
using eSecurity.Security.Authentication.Odic.Client;

namespace eSecurity.Features.Connect.Queries;

public sealed record GetClientScopesQuery(string ClientId) : IRequest<Result>;

public class GetClientScopesQueryHandler(
    IClientManager clientManager) : IRequestHandler<GetClientScopesQuery, Result>
{
    private readonly IClientManager clientManager = clientManager;

    public async Task<Result> Handle(GetClientScopesQuery request, CancellationToken cancellationToken)
    {
        var client = await clientManager.FindByClientIdAsync(request.ClientId, cancellationToken);
        if (client is null) return Results.NotFound("Client was not found.");

        var response = client.AllowedScopes
            .Select(allowedScope => allowedScope.Scope)
            .Select(scope => new ScopeDto()
            {
                Name = scope.Name,
                Description = scope.Description,
            }).ToList();
        
        return Result.Success(response);
    }
}