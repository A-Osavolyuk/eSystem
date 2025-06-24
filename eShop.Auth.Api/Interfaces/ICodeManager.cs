namespace eShop.Auth.Api.Interfaces;

public interface ICodeManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, CodeType codeType, CancellationToken cancellationToken = default);
    public ValueTask<CodeEntity?> FindAsync(UserEntity user, SenderType sender, CodeType type, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, string code, SenderType sender, CodeType type, CancellationToken cancellationToken = default);
}