namespace eShop.Auth.Api.Interfaces;

public interface ICodeManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, CodeType codeType, CancellationToken cancellationToken = default);
    public ValueTask<VerificationCodeEntity?> FindAsync(UserEntity user, string code, CancellationToken cancellationToken = default);
    public ValueTask<VerificationCodeEntity?> FindAsync(UserEntity user, CodeType type, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, string code, CodeType type, CancellationToken cancellationToken = default);
    public ValueTask DeleteAsync(VerificationCodeEntity entity, CancellationToken cancellationToken = default);
}