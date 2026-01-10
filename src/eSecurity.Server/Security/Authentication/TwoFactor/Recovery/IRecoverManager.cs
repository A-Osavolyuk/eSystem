using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.TwoFactor.Recovery;

public interface IRecoverManager
{
    public ValueTask<List<string>> UnprotectAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<List<string>> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, string code, CancellationToken cancellationToken = default);
    public ValueTask<Result> RevokeAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<bool> HasAsync(UserEntity user, CancellationToken cancellationToken = default);
}