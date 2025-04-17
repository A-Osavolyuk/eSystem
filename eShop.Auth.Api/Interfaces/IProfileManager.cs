namespace eShop.Auth.Api.Interfaces;

public interface IProfileManager
{
    public ValueTask<PersonalData?> FindAsync(AppUser user, CancellationToken cancellationToken = default);

    public ValueTask<IdentityResult> SetAsync(AppUser user, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default);

    public ValueTask<IdentityResult> UpdateAsync(AppUser user, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default);

    public ValueTask<IdentityResult> DeleteAsync(AppUser user,
        CancellationToken cancellationToken = default);
}