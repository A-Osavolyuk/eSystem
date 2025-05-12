using eShop.Auth.Api.Enums;

namespace eShop.Auth.Api.Data.Seed;

public class PermissionSeed : Seed<PermissionEntity, Guid>
{
    public override List<PermissionEntity> Get()
    {
        return
        [
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
            new()
            {
                Id = Guid.Parse("0e572d3d-b4a1-4dc8-8c3b-c52c86a61a45"),
                ResourceId = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"), 
                Name = "Role:Assign",
                Action = ActionType.Assign
            },
            new()
            {
                Id = Guid.Parse("80f1dc43-fd49-4c57-8d30-635f5f5a9f37"),
                ResourceId = Guid.Parse("fe216bfd-65af-404a-b30e-60465fca02ee"), 
                Name = "Role:Unassign",
                Action = ActionType.Unassign
            },
            new()
            {
                Id = Guid.Parse("3be35bbf-bc09-4ccf-a4d5-6211f9b09a90"),
                ResourceId = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"), 
                Name = "Permission:Grant",
                Action = ActionType.Grant
            },
            new()
            {
                Id = Guid.Parse("65b6506b-52bc-4f9f-9741-37b1059e1d7c"),
                ResourceId = Guid.Parse("b8c49fab-8e40-431d-8edc-3a2e17eeb2e6"), 
                Name = "Permission:Revoke",
                Action = ActionType.Revoke
            },
            new()
            {
                Id = Guid.Parse("a2b77f86-80e9-468f-b875-d63087f9a68f"),
                ResourceId = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"), 
                Name = "User:Invite",
                Action = ActionType.Invite
            },
            new()
            {
                Id = Guid.Parse("d4aeea79-58d1-4a66-8f02-9d3c9c40e2f7"),
                ResourceId = Guid.Parse("e3fef8c4-0105-419a-b889-66b9f1036e8c"), 
                Name = "User:Remove",
                Action = ActionType.Remove
            },
            new()
            {
                Id = Guid.Parse("65b45022-3073-497f-9950-1fa2cfb8c212"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "Admin:Read",
                Action = ActionType.Read
            },
            new()
            {
                Id = Guid.Parse("4a68513a-b7ad-46ff-b5f3-f8d0c9de2411"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "Admin:Create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.Parse("6e3f3ff1-bd34-4b4e-b415-6f04227bfb57"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "Admin:Update",
                Action = ActionType.Update
            },
            new()
            {
                Id = Guid.Parse("f31c56b5-ec0d-4191-8f9f-5450511b3786"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "Admin:Delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.Parse("a4e12251-2d6d-4b0d-9053-2d56fcd8ac9b"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "Admin:All",
                Action = ActionType.All
            }
        ];
    }
}