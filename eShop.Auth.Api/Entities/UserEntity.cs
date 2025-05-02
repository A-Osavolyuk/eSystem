namespace eShop.Auth.Api.Entities;

public class UserEntity : IdentityUser<Guid>, IEntity
{
    public PersonalDataEntity? PersonalData { get; init; }
    public SecurityTokenEntity? AuthenticationToken { get; init; }
    public ICollection<UserPermissionsEntity> Permissions { get; init; } = null!;
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}