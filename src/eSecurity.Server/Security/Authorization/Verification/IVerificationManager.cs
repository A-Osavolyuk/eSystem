using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Verification;

public interface IVerificationManager
{
    public ValueTask<VerificationRequestEntity?> FindByIdAsync(Guid id, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(UserEntity user, PurposeType purpose,
        ActionType action, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(VerificationRequestEntity request, 
        CancellationToken cancellationToken = default);

    public ValueTask<Result> VerifyAsync(UserEntity user, PurposeType purpose,
        ActionType action, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> ConsumeAsync(VerificationRequestEntity request, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CancelAsync(VerificationRequestEntity request, 
        CancellationToken cancellationToken = default);
}