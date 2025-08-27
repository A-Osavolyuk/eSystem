namespace eShop.Auth.Api.Interfaces;

public interface IVerificationManager
{
    public ValueTask<Result> CreateAsync(UserEntity user, CodeResource resource, 
        CodeType type, CancellationToken cancellationToken);
    
    public ValueTask<Result> VerifyAsync(UserEntity user, CodeResource resource, 
        CodeType type, CancellationToken cancellationToken);
}