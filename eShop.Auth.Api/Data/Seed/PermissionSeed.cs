namespace eShop.Auth.Api.Data.Seed;

public class PermissionSeed : Seed<PermissionEntity, Guid>
{
    protected override List<PermissionEntity> Get()
    {
        return
        [
            new () { Id = Guid.Parse("dba6e723-ac0f-42a3-91fd-e40bdb08e26b"), Name = "Permission.Account.ManageAccount" },
            new () { Id = Guid.Parse("349898ee-1f26-4877-86ca-0960361b5e3e"), Name = "Permission.Admin.ManageUsers" },
            new () { Id = Guid.Parse("74e0644b-6f9d-4964-a9a6-341a7834cc0e"), Name = "Permission.Admin.ManageLockout" },
            new () { Id = Guid.Parse("e14d7bcf-0ab4-4168-b2b5-ff0894782097"), Name = "Permission.Admin.ManageRoles" },
            new () { Id = Guid.Parse("df258394-6290-43b8-abc9-d52aba8ff6e6"), Name = "Permission.Admin.ManagePermissions" },
            new () { Id = Guid.Parse("3c38ecbf-a14c-4d46-9eab-6b297cca124d"), Name = "Permission.Product.View" },
            new () { Id = Guid.Parse("5034df8e-c656-4f85-b197-7afff97ecad0"), Name = "Permission.Product.Edit" },
            new () { Id = Guid.Parse("25af1455-d0b8-4be3-b6ff-9cf393d59258"), Name = "Permission.Product.Delete" },
            new () { Id = Guid.Parse("a1216fa3-66dd-4a6d-8616-48a7b9900649"), Name = "Permission.Product.Create" }
        ];
    }
}