using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Consents;

public interface IConsentManager
{
    public ValueTask<ConsentEntity?> FindByIdAsync(Guid id, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<ConsentEntity?> FindAsync(UserEntity user, ClientEntity client, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(ConsentEntity consent, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> GrantAsync(ConsentEntity consent, ClientAllowedScopeEntity scope, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> RevokeAsync(ConsentEntity consent, ClientAllowedScopeEntity scope, 
        CancellationToken cancellationToken = default);
}