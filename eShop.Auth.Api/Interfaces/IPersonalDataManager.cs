namespace eShop.Auth.Api.Interfaces;

public interface IPersonalDataManager
{
    public ValueTask<PersonalDataEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(PersonalDataEntity personalData, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(PersonalDataEntity personalData, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(PersonalDataEntity personalData, CancellationToken cancellationToken = default);
}