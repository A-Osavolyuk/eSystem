using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

public class ScopeSeed : Seed<ScopeEntity>
{
    public override List<ScopeEntity> Get()
    {
        return
        [
            new ScopeEntity()
            {
                Id = Guid.Parse("2475899a-e18c-4563-a598-c579113dbda4"),
                Name = "openid",
                Description = "openid",
                CreateDate = DateTimeOffset.UtcNow
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("7c35e89d-9276-4b3a-9c9a-4b37e9ef137a"),
                Name = "profile",
                Description = "profile",
                CreateDate = DateTimeOffset.UtcNow
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("26d364e7-be16-4d72-9304-28b141a6e118"),
                Name = "email",
                Description = "email",
                CreateDate = DateTimeOffset.UtcNow
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("4026c2dd-df54-4591-bacf-1b8f446528c5"),
                Name = "address",
                Description = "address",
                CreateDate = DateTimeOffset.UtcNow
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("8d2269e7-ae28-4911-b5ef-6f47418fb65e"),
                Name = "phone-number",
                Description = "phone-number",
                CreateDate = DateTimeOffset.UtcNow
            },
        ];
    }
}