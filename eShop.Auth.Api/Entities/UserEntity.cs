namespace eShop.Auth.Api.Entities;

public class UserEntity : IdentityUser<Guid>, IEntity
{
    public Guid PersonalDataId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    
    public PersonalDataEntity? PersonalData { get; init; }
    public ICollection<UserPermissionsEntity> Permissions { get;  init; } = null!;
}