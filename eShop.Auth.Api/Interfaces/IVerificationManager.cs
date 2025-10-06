namespace eShop.Auth.Api.Interfaces;

public interface IVerificationManager
{
    public ValueTask<Result> CreateAsync(UserEntity user, CodeResource resource, 
        CodeType type, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> VerifyAsync(UserEntity user, CodeResource resource, 
        CodeType type, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> SubscribeAsync(UserEntity user, VerificationMethod method, 
        bool isPrimary = false, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UnsubscribeAsync(UserVerificationMethodEntity method, 
        CancellationToken cancellationToken = default);
}