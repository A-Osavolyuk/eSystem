using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public sealed class ScopeSeed : Seed<ScopeEntity>
{
    public override List<ScopeEntity> Get()
    {
        return
        [
            new ScopeEntity()
            {
                Id = Guid.Parse("4f49143c-14b4-435c-aa36-9654a9944f63"),
                Description = "Allows OpenIdConnect extension",
                Value = "openid"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("eeb1769f-f45e-43f8-b781-790a80eeb952"),
                Description = "Allows offline access flow (token refresh)",
                Value = "offline_access"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("4de6ba7a-1db7-4f52-9483-97f4a1906fe8"),
                Description = "Allows clients to initiate DCR flow",
                Value = "client_registration"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("33721238-6e31-4cd3-9457-11316dbf5eeb"),
                Description = "Allows profile-related claims be present in JWT tokens",
                Value = "profile"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("d3bd9531-e4b9-4a6c-9706-c7c3957db095"),
                Description = "Allows email claim be present in JWT tokens",
                Value = "email"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("f067c039-ee63-44bb-b224-00a5872dfe28"),
                Description = "Allows phone claim be present in JWT tokens",
                Value = "phone"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("e36782cf-c927-40c7-97d5-3fcc75fd211f"),
                Description = "Allows address claim be present in JWT tokens",
                Value = "address"
            }
        ];
    }
}