using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSystem.Core.Data.Seeding;

namespace eSecurity.Server.Data.Seeding;

public class ClientUriSeed : Seed<ClientUriEntity>
{
    public override List<ClientUriEntity> Get()
    {
        return
        [
            // eSecurity.Client
            new ClientUriEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.Redirect,
                Uri = "https://localhost:6501/connect/callback",
            },
            new ClientUriEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.PostLogoutRedirect,
                Uri = "https://localhost:6501/connect/logged-out",
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.FrontChannelLogout,
                Uri = "https://localhost:6501/connect/logout/callback",
            },

            // eCinema.Client
            new ClientUriEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Type = UriType.Redirect,
                Uri = "https://localhost:6204/api/v1/connect/callback",
            },
            new ClientUriEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Type = UriType.PostLogoutRedirect,
                Uri = "https://localhost:6502/connect/logged-out",
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Type = UriType.FrontChannelLogout,
                Uri = "https://localhost:6502/connect/logout/callback",
            }
        ];
    }
}