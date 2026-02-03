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
                Name = "OpenIdConnect",
                Description = "Allows OpenIdConnect extension",
                Value = "openid"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("eeb1769f-f45e-43f8-b781-790a80eeb952"),
                Name = "Offline access",
                Description = "Allows offline access flow (token refresh)",
                Value = "offline_access"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("4de6ba7a-1db7-4f52-9483-97f4a1906fe8"),
                Name = "Dynamic Client Registration (DCR)",
                Description = "Allows clients to initiate DCR flow",
                Value = "client_registration"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("33721238-6e31-4cd3-9457-11316dbf5eeb"),
                Name = "Profile",
                Description = "Allows profile-related claims be present in JWT tokens",
                Value = "profile"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("d3bd9531-e4b9-4a6c-9706-c7c3957db095"),
                Name = "Email",
                Description = "Allows email claim be present in JWT tokens",
                Value = "email"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("f067c039-ee63-44bb-b224-00a5872dfe28"),
                Name = "Phone",
                Description = "Allows phone claim be present in JWT tokens",
                Value = "phone"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("e36782cf-c927-40c7-97d5-3fcc75fd211f"),
                Name = "Address",
                Description = "Allows address claim be present in JWT tokens",
                Value = "address"
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("8b397566-6fed-4cdc-8daa-4dc5d3a56a57"),
                Name = "Read access",
                Description = "Client read access to specified resource",
                Value = "*:read",
                IsTemplate = true
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("52784bd5-e5be-4275-9b77-15061d42ec00"),
                Name = "Write access",
                Description = "Client write access to specified resource",
                Value = "*:write",
                IsTemplate = true
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("2e26da33-2356-4851-b2fd-23e54487e9e7"),
                Name = "Admin access",
                Description = "Client admin access to specified resource",
                Value = "*:admin",
                IsTemplate = true
            },
        ];
    }
}