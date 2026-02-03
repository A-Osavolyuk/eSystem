using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

public interface IClientManager
{
    public ValueTask<List<ClientEntity>> GetClientsAsync(SessionEntity session, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<ClientEntity?> FindByIdAsync(string clientId, CancellationToken cancellationToken = default);
    public ValueTask<ClientEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<List<string>> GetAudiencesAsync(CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(ClientEntity entity, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> RelateAsync(ClientEntity client, SessionEntity session,
        CancellationToken cancellationToken = default);
}