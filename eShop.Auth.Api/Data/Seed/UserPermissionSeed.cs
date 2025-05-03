namespace eShop.Auth.Api.Data.Seed;

public class UserPermissionSeed : Seed<UserPermissionsEntity, Guid>
{
    protected override List<UserPermissionsEntity> Get()
    {
        return
        [
            new ()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                Id = Guid.Parse("349898ee-1f26-4877-86ca-0960361b5e3e")
            },
            new ()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                Id = Guid.Parse("74e0644b-6f9d-4964-a9a6-341a7834cc0e")
            },
            new ()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                Id = Guid.Parse("e14d7bcf-0ab4-4168-b2b5-ff0894782097")
            },
            new ()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                Id = Guid.Parse("df258394-6290-43b8-abc9-d52aba8ff6e6")
            },
            new ()
            {
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                Id = Guid.Parse("dba6e723-ac0f-42a3-91fd-e40bdb08e26b")
            }
        ];
    }
}