namespace eShop.Auth.Api.Interfaces;

public interface IProfileManager
{
    public ValueTask<PersonalData?> FindAsync(UserEntity userEntity, CancellationToken cancellationToken = default);

    public ValueTask<IdentityResult> SetAsync(UserEntity userEntity, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default);

    public ValueTask<IdentityResult> UpdateAsync(UserEntity userEntity, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default);

    public ValueTask<IdentityResult> DeleteAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default);
}