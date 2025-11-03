using eSecurity.Data.Entities;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Security.Authorization.Access;

public interface IVerificationManager
{
    public ValueTask<Result> CreateAsync(UserEntity user, PurposeType purpose, 
        ActionType action, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> VerifyAsync(UserEntity user, PurposeType purpose, 
        ActionType action, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> SubscribeAsync(UserEntity user, VerificationMethod method, 
        bool preferred = false, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UnsubscribeAsync(UserVerificationMethodEntity method, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> PreferAsync(UserEntity user, VerificationMethod method, 
        CancellationToken cancellationToken = default);
}