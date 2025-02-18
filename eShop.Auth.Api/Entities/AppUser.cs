namespace eShop.Auth.Api.Entities;

public class AppUser : IdentityUser, IAuditable
{
    public PersonalDataEntity? PersonalData { get; init; }
    public SecurityTokenEntity? AuthenticationToken { get; init; }
    public ICollection<UserPermissionsEntity> Permissions { get; init; } = null!;
    public DateTime CreateDate { get; init; }
    public DateTime UpdateDate { get; init; }
}