namespace eShop.Auth.Api.Interfaces;

public interface ICodeManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, CodeType codeType, CancellationToken cancellationToken = default);
    public ValueTask<CodeEntity?> FindAsync(UserEntity user, CodeType type, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, string code, CodeType type, CancellationToken cancellationToken = default);
    public ValueTask DeleteAsync(CodeEntity entity, CancellationToken cancellationToken = default);
}