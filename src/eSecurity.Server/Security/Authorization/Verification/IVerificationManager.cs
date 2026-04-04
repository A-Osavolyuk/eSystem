using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Verification;

public interface IVerificationManager
{
    public ValueTask<VerificationRequestEntity?> FindByIdAsync(Guid id, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(VerificationRequestEntity request, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> ConsumeAsync(VerificationRequestEntity request, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CancelAsync(VerificationRequestEntity request, 
        CancellationToken cancellationToken = default);
}