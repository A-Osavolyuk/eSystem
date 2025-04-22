namespace eShop.Auth.Api.Interfaces;

public interface ICodeManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, Verification codeType, CancellationToken cancellationToken = default);
    public ValueTask<VerificationCodeEntity?> FindAsync(UserEntity user, string code, CancellationToken cancellationToken = default);
    public ValueTask<VerificationCodeEntity?> FindAsync(UserEntity user, Verification type, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, string code, Verification type, CancellationToken cancellationToken = default);
    public Result Validate(VerificationCodeEntity entity);
    public ValueTask DeleteAsync(VerificationCodeEntity entity, CancellationToken cancellationToken = default);
}