namespace eShop.Auth.Api.Interfaces;

public interface IVerificationCodeManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, CodeType codeType, CodeResource resource,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> VerifyAsync(UserEntity user, string code, SenderType sender, CodeType type,
        CodeResource resource, CancellationToken cancellationToken = default);
}