using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Client;

public interface IClientQueryService
{
    ValueTask<List<ClientEntity>> ListBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    ValueTask<ClientEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<ClientEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    ValueTask<List<ClientResponseTypeEntity>> GetSupportedResponseTypesAsync(Guid clientId,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientTokenAuthMethodEntity>> GetSupportedTokenAuthMethodsAsync(Guid clientId,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientAllowedScopeEntity>> GetAllowedScopesAsync(Guid clientId,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientGrantTypeEntity>> GetSupportedGrantTypesAsync(Guid clientId,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientAudienceEntity>> GetSupportedAudiencesAsync(Guid clientId,
        CancellationToken cancellationToken = default);

    ValueTask<List<ClientUriEntity>> GetUrisAsync(Guid clientId, CancellationToken cancellationToken = default);
}