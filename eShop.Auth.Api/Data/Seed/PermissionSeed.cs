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
                Name = "account:read",
                Action = ActionType.Read
            },
            new()
            {
                Id = Guid.Parse("afac8991-f6f4-42d3-a842-d5fa79959451"),
                ResourceId = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"), 
                Name = "account:create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.Parse("c8e5b8ff-2ab6-45f9-a731-0f5e9a129e77"),
                ResourceId = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"), 
                Name = "account:update",
                Action = ActionType.Update
            },
            new()
            {
                Id = Guid.Parse("510a5b4c-ff4f-428d-a913-34e7a49ed19b"),
                ResourceId = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"), 
                Name = "account:delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.Parse("e61e3480-4e63-4f42-bb3f-9744415036cb"),
                ResourceId = Guid.Parse("1a06eabb-3354-4b03-be52-ab42743eaa97"), 
                Name = "account:all",
                Action = ActionType.All
            },
            new()
            {
                Id = Guid.Parse("65b45022-3073-497f-9950-1fa2cfb8c212"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "admin:read",
                Action = ActionType.Read
            },
            new()
            {
                Id = Guid.Parse("4a68513a-b7ad-46ff-b5f3-f8d0c9de2411"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "admin:create",
                Action = ActionType.Create
            },
            new()
            {
                Id = Guid.Parse("6e3f3ff1-bd34-4b4e-b415-6f04227bfb57"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "admin:update",
                Action = ActionType.Update
            },
            new()
            {
                Id = Guid.Parse("f31c56b5-ec0d-4191-8f9f-5450511b3786"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "admin:delete",
                Action = ActionType.Delete
            },
            new()
            {
                Id = Guid.Parse("a4e12251-2d6d-4b0d-9053-2d56fcd8ac9b"),
                ResourceId = Guid.Parse("70969390-c949-4426-b3c1-7fd86580b62e"), 
                Name = "admin:all",
                Action = ActionType.All
            }
        ];
    }
}