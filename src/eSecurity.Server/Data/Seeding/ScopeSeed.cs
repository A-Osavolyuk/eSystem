using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using Microsoft.OpenApi.Writers;

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
                Value = ScopeTypes.OpenId
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("eeb1769f-f45e-43f8-b781-790a80eeb952"),
                Description = "Allows offline access flow (token refresh)",
                Value = ScopeTypes.OfflineAccess
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("4de6ba7a-1db7-4f52-9483-97f4a1906fe8"),
                Description = "Allows clients to initiate DCR flow",
                Value = ScopeTypes.ClientRegistration
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("ca265929-4c5c-41fd-b509-dacaa5c33586"),
                Description = "Allows clients to initiate a transformation flow of token exchange",
                Value = ScopeTypes.Transformation
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("74be25d7-c054-4d40-a9b9-a3913c4e1b5e"),
                Description = "Allows clients to initiate a delegation flow of token exchange",
                Value = ScopeTypes.Delegation
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("33721238-6e31-4cd3-9457-11316dbf5eeb"),
                Description = "Allows profile-related claims be present in JWT tokens",
                Value = ScopeTypes.Profile
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("d3bd9531-e4b9-4a6c-9706-c7c3957db095"),
                Description = "Allows email claim be present in JWT tokens",
                Value = ScopeTypes.Email
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("f067c039-ee63-44bb-b224-00a5872dfe28"),
                Description = "Allows phone claim be present in JWT tokens",
                Value = ScopeTypes.Phone
            },
            new ScopeEntity()
            {
                Id = Guid.Parse("e36782cf-c927-40c7-97d5-3fcc75fd211f"),
                Description = "Allows address claim be present in JWT tokens",
                Value = ScopeTypes.Address
            },
        ];
    }
}