namespace eShop.Auth.Api.Data.Seed;

public class RolePermissionSeed : Seed<RolePermissionEntity>
{
    public override List<RolePermissionEntity> Get()
    {
        return
        [
            // Admin access
            new() { RoleId = Guid.Parse("e6d15d97-b803-435a-9dc2-a7c45c08a1af"), PermissionId = Guid.Parse("e61e3480-4e63-4f42-bb3f-9744415036cb") },
            
            // User access
            new() { RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"), PermissionId = Guid.Parse("b2c8c2b7-0c88-47cd-9870-1638c1b022c3") },
            new() { RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"), PermissionId = Guid.Parse("afac8991-f6f4-42d3-a842-d5fa79959451") },
            new() { RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"), PermissionId = Guid.Parse("c8e5b8ff-2ab6-45f9-a731-0f5e9a129e77") },
            new() { RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"), PermissionId = Guid.Parse("510a5b4c-ff4f-428d-a913-34e7a49ed19b") },
        ];
    }
}