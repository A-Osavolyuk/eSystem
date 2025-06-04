namespace eShop.Auth.Api.Interfaces;

public interface IPersonalDataManager
{
    public ValueTask<PersonalDataEntity?> FindAsync(UserEntity userEntity, CancellationToken cancellationToken = default);

    public ValueTask<Result> SetAsync(UserEntity userEntity, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> UpdateAsync(UserEntity userEntity, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> DeleteAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default);
}