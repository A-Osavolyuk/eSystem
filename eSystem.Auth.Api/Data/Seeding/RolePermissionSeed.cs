using eSystem.Domain.Abstraction.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

public class RolePermissionSeed : Seed<RolePermissionEntity>
{
    public override List<RolePermissionEntity> Get()
    {
        return
        [
            // User access
            new() { RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"), PermissionId = Guid.Parse("b2c8c2b7-0c88-47cd-9870-1638c1b022c3") },
            new() { RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"), PermissionId = Guid.Parse("afac8991-f6f4-42d3-a842-d5fa79959451") },
            new() { RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"), PermissionId = Guid.Parse("c8e5b8ff-2ab6-45f9-a731-0f5e9a129e77") },
            new() { RoleId = Guid.Parse("270910a1-d582-4ce0-8b23-c8141d720064"), PermissionId = Guid.Parse("510a5b4c-ff4f-428d-a913-34e7a49ed19b") },
            
            // Seller access
            new() { RoleId = Guid.Parse("370b1519-9e73-4676-814c-e21433ceb424"), PermissionId = Guid.Parse("2fb162b2-5e7b-4af3-8d9d-08d48ed94d31") },
            new() { RoleId = Guid.Parse("370b1519-9e73-4676-814c-e21433ceb424"), PermissionId = Guid.Parse("39848c79-7f8d-4abb-9f81-ee25193a5ee5") },
            new() { RoleId = Guid.Parse("370b1519-9e73-4676-814c-e21433ceb424"), PermissionId = Guid.Parse("1473d3b4-ea69-4637-b35e-b3e87d3f8d87") },
            
            // Moderator access
            new() { RoleId = Guid.Parse("52425482-b33d-493f-acb2-aa520d73ce95"), PermissionId = Guid.Parse("791fabb6-8370-4dcf-8adf-693e478404c4") },
            new() { RoleId = Guid.Parse("52425482-b33d-493f-acb2-aa520d73ce95"), PermissionId = Guid.Parse("1f523f32-a423-4f84-ab2f-00dc568aaa62") },
            new() { RoleId = Guid.Parse("52425482-b33d-493f-acb2-aa520d73ce95"), PermissionId = Guid.Parse("3ac2c17e-02cd-40a9-991b-df068b18a0aa") },
            new() { RoleId = Guid.Parse("52425482-b33d-493f-acb2-aa520d73ce95"), PermissionId = Guid.Parse("cd00c3b1-201a-4e25-ae09-d2ad9a81122b") },
            new() { RoleId = Guid.Parse("52425482-b33d-493f-acb2-aa520d73ce95"), PermissionId = Guid.Parse("28974878-6f22-4abb-8a73-efd12f7f65b4") },
            new() { RoleId = Guid.Parse("52425482-b33d-493f-acb2-aa520d73ce95"), PermissionId = Guid.Parse("0988be01-70fd-4408-b78c-a573492a975c") },
        ];
    }
}