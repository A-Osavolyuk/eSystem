using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;

namespace eSecurity.Server.Data.Seeding;

public class ClientSeed : Seed<ClientEntity>
{
    public override List<ClientEntity> Get()
    {
        return
        [
            new ClientEntity
            {
                Id = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Secret = "09ba08a500f11aa321fb1dfd8c40ed017e2c1b70f992b86dac6d591089e33cb2",
                Name = "eSecurity",
                Audience = "eSecurity",
                RequireClientSecret = true,
                AllowOfflineAccess = true,
                RefreshTokenRotationEnabled = false,
                RefreshTokenLifetime = TimeSpan.FromDays(30),
                ClientType = ClientType.Confidential,
                AccessTokenType = AccessTokenType.Jwt,
                SubjectType = SubjectType.Public,
            },
            new ClientEntity
            {
                Id = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Secret = "09ba08a500f11aa321fb1dfd8c40ed017e2c1b70f992b86dac6d591089e33cb2",
                Name = "eCinema",
                Audience = "eCinema",
                RequireClientSecret = true,
                AllowOfflineAccess = true,
                RefreshTokenRotationEnabled = false,
                RefreshTokenLifetime = TimeSpan.FromDays(30),
                ClientType = ClientType.Confidential,
                AccessTokenType = AccessTokenType.Jwt,
                SubjectType = SubjectType.Public,
            },
            new ClientEntity
            {
                Id = Guid.Parse("fc1c1662-cd80-4fab-b924-a39168765558"),
                Secret = "09ba08a500f11aa321fb1dfd8c40ed017e2c1b70f992b86dac6d591089e33cb2",
                Name = "eMessage",
                Audience = "eMessage",
                RequireClientSecret = true,
                AllowOfflineAccess = false,
                RefreshTokenRotationEnabled = false,
                RefreshTokenLifetime = TimeSpan.FromDays(30),
                ClientType = ClientType.Confidential,
                AccessTokenType = AccessTokenType.Reference,
                SubjectType = SubjectType.Public,
            }
        ];
    }
}