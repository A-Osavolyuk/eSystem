namespace eShop.Auth.Api.Data.Seed;

public class UserRoleSeed : Seed<UserRoleEntity>
{
    public override List<UserRoleEntity> Get()
    {
        return [
            new ()
            {
                RoleId = Guid.Parse("e6d15d97-b803-435a-9dc2-a7c45c08a1af"),
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3")
            },
            new ()
            {
                RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"),
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3")
            }
        ];
    }
}