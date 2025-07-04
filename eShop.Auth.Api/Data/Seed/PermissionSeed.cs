namespace eShop.Auth.Api.Data.Seed;

public class PermissionSeed : Seed<PermissionEntity>
{
    public override List<PermissionEntity> Get()
    {
        return
        [
            //Account permissions
            new()
            {
                Id = Guid.Parse("b2c8c2b7-0c88-47cd-9870-1638c1b022c3"),
                ResourceId = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"), 
                Name = "Account:Read",
                Action = ActionType.Read
            },
            new()
            {
                Id = Guid.Parse("afac8991-f6f4-42d3-a842-d5fa79959451"),
                ResourceId = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"), 
                Name = "Account:Create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.Parse("c8e5b8ff-2ab6-45f9-a731-0f5e9a129e77"),
                ResourceId = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"), 
                Name = "Account:Update",
                Action = ActionType.Update
            },
            new()
            {
                Id = Guid.Parse("510a5b4c-ff4f-428d-a913-34e7a49ed19b"),
                ResourceId = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"), 
                Name = "Account:Delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.Parse("e61e3480-4e63-4f42-bb3f-9744415036cb"),
                ResourceId = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"), 
                Name = "Account:All",
                Action = ActionType.All
            },
            
            //Role permissions
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"), 
                Name = "Role:Assign",
                Action = ActionType.Assign
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"), 
                Name = "Role:Unassign",
                Action = ActionType.Unassign
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"), 
                Name = "Role:Read",
                Action = ActionType.Read
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"), 
                Name = "Role:Create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"), 
                Name = "Role:Delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"), 
                Name = "Role:Update",
                Action = ActionType.Update
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"), 
                Name = "Role:All",
                Action = ActionType.All
            },
            
            //Permissions permissions
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"), 
                Name = "Permission:Grant",
                Action = ActionType.Grant
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"), 
                Name = "Permission:Revoke",
                Action = ActionType.Revoke
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"), 
                Name = "Permission:Read",
                Action = ActionType.Read
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"), 
                Name = "Permission:Create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"), 
                Name = "Permission:Delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"), 
                Name = "Permission:Update",
                Action = ActionType.Update
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"), 
                Name = "Permission:All",
                Action = ActionType.All
            },
            
            //User permissions
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"), 
                Name = "User:Read",
                Action = ActionType.Read
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"), 
                Name = "User:Create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"), 
                Name = "User:Update",
                Action = ActionType.Update
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"), 
                Name = "User:Delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"), 
                Name = "User:Lockout",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"), 
                Name = "User:Unlock",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ResourceId = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"), 
                Name = "User:All",
                Action = ActionType.All
            },
            
            //Brand permissions
            new()
            {
                Id = Guid.Parse("cd00c3b1-201a-4e25-ae09-d2ad9a81122b"),
                ResourceId = Guid.Parse("5201d4b1-d6bf-488b-b925-7a5f8d1c8a0d"), 
                Name = "Brand:Create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.Parse("28974878-6f22-4abb-8a73-efd12f7f65b4"),
                ResourceId = Guid.Parse("5201d4b1-d6bf-488b-b925-7a5f8d1c8a0d"), 
                Name = "Brand:Delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.Parse("0988be01-70fd-4408-b78c-a573492a975c"),
                ResourceId = Guid.Parse("5201d4b1-d6bf-488b-b925-7a5f8d1c8a0d"), 
                Name = "Brand:Update",
                Action = ActionType.Update
            },
            
            //Supplier permissions
            new()
            {
                Id = Guid.Parse("791fabb6-8370-4dcf-8adf-693e478404c4"),
                ResourceId = Guid.Parse("b4c8a3dc-ca22-4972-a4bc-ac6899936231"), 
                Name = "Supplier:Create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.Parse("1f523f32-a423-4f84-ab2f-00dc568aaa62"),
                ResourceId = Guid.Parse("b4c8a3dc-ca22-4972-a4bc-ac6899936231"), 
                Name = "Supplier:Delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.Parse("3ac2c17e-02cd-40a9-991b-df068b18a0aa"),
                ResourceId = Guid.Parse("b4c8a3dc-ca22-4972-a4bc-ac6899936231"), 
                Name = "Supplier:Update",
                Action = ActionType.Update
            },
            
            //Product permissions
            new()
            {
                Id = Guid.Parse("2fb162b2-5e7b-4af3-8d9d-08d48ed94d31"),
                ResourceId = Guid.Parse("9120cce8-d123-4181-8a53-baaa7774599b"), 
                Name = "Product:Create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.Parse("39848c79-7f8d-4abb-9f81-ee25193a5ee5"),
                ResourceId = Guid.Parse("9120cce8-d123-4181-8a53-baaa7774599b"), 
                Name = "Product:Delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.Parse("1473d3b4-ea69-4637-b35e-b3e87d3f8d87"),
                ResourceId = Guid.Parse("9120cce8-d123-4181-8a53-baaa7774599b"), 
                Name = "Product:Update",
                Action = ActionType.Update
            },
        ];
    }
}