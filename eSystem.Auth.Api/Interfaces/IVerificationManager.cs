using eSystem.Auth.Api.Entities;
using eSystem.Domain.Security.Verification;

namespace eSystem.Auth.Api.Interfaces;

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