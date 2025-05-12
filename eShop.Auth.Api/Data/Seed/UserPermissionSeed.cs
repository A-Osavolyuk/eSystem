namespace eShop.Auth.Api.Data.Seed;

public class UserPermissionSeed : Seed<UserPermissionsEntity>
{
    public override List<UserPermissionsEntity> Get()
    {
        return
        [
            new ()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                PermissionId = Guid.Parse("e61e3480-4e63-4f42-bb3f-9744415036cb")
            },
            new ()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                PermissionId = Guid.Parse("a4e12251-2d6d-4b0d-9053-2d56fcd8ac9b")
            }
        ];
    }
}