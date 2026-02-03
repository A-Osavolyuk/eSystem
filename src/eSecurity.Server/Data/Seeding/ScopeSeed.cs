using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Data.Seeding;

public class ScopeSeed : Seed<ScopeEntity>
{
    public override List<ScopeEntity> Get()
    {
        return
        [
            new ScopeEntity
            {
                Id = Guid.Parse("2475899a-e18c-4563-a598-c579113dbda4"),
                Name = ScopesType.OpenId,
                Description = ScopesType.OpenId
            },
            new ScopeEntity
            {
                Id = Guid.Parse("7c35e89d-9276-4b3a-9c9a-4b37e9ef137a"),
                Name = ScopesType.Profile,
                Description = ScopesType.Profile
            },
            new ScopeEntity
            {
                Id = Guid.Parse("26d364e7-be16-4d72-9304-28b141a6e118"),
                Name = ScopesType.Email,
                Description = ScopesType.Email
            },
            new ScopeEntity
            {
                Id = Guid.Parse("4026c2dd-df54-4591-bacf-1b8f446528c5"),
                Name = ScopesType.Address,
                Description = ScopesType.Address
            },
            new ScopeEntity
            {
                Id = Guid.Parse("8d2269e7-ae28-4911-b5ef-6f47418fb65e"),
                Name = ScopesType.Phone,
                Description = ScopesType.Phone
            },
            new ScopeEntity
            {
                Id = Guid.Parse("865d1609-76c6-4928-a2f0-d1ca77f1498b"),
                Name = ScopesType.OfflineAccess,
                Description = ScopesType.OfflineAccess
            },
        ];
    }
}