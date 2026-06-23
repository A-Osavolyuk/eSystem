using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Client;

public interface IClientQueryService
{
    ValueTask<List<ClientEntity>> ListBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    ValueTask<ClientEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<ClientEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    ValueTask<List<ClientResponseTypeEntity>> GetSupportedResponseTypesAsync(ClientEntity client,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientTokenAuthMethodEntity>> GetSupportedTokenAuthMethodsAsync(ClientEntity client,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientAllowedScopeEntity>> GetAllowedScopesAsync(ClientEntity client,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientGrantTypeEntity>> GetSupportedGrantTypesAsync(ClientEntity client,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientAudienceEntity>> GetSupportedAudiencesAsync(ClientEntity client,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientUriEntity>> GetUrisAsync(ClientEntity client, CancellationToken cancellationToken = default);
}