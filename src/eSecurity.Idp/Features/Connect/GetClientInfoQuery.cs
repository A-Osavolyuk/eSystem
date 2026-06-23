using eSecurity.Core.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Idp.Security.Authentication.Client;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Connect;

public record GetClientInfoQuery(string ClientId) : IRequest<Result>;

public class GetClientInfoQueryHandler(IClientQueryService clientQueryService) : IRequestHandler<GetClientInfoQuery, Result>
{
    private readonly IClientQueryService _clientQueryService = clientQueryService;

    public async Task<Result> Handle(GetClientInfoQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientQueryService.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Client not found"
            });
        }

        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(
            client, cancellationToken);
        
        var response = new ClientInfo
        {
            ClientName = client.Name,
            ClientType = client.ClientType,
            RequiredScopes = clientScopes.Select(x => x.Scope.Value).ToList(),
        };
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}