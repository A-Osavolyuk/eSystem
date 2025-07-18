namespace eShop.Auth.Api.Interfaces;

public interface IReasonManager
{
    public ValueTask<List<LockoutReasonEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<LockoutReasonEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<LockoutReasonEntity?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
    public ValueTask<LockoutReasonEntity?> FindByTypeAsync(LockoutType type, CancellationToken cancellationToken = default);
}