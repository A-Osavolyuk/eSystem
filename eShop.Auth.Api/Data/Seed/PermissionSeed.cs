namespace eShop.Auth.Api.Data.Seed;

public class PermissionSeed : Seed<PermissionEntity, Guid>
{
    public override List<PermissionEntity> Get()
    {
        return
        [
            #region Account

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

            #endregion

            #region Roles

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

            #endregion

            #region Permission

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

            #endregion

            #region Users

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

            #endregion
        ];
    }
}