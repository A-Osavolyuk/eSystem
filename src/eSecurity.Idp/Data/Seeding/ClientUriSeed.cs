using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSystem.Core.Server.Data.Seeding;

namespace eSecurity.Idp.Data.Seeding;

public class ClientUriSeed : Seed<ClientUriEntity>
{
    public override List<ClientUriEntity> Get()
    {
        return
        [
            // eSecurity.Client
            new ClientUriEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.Redirect,
                Uri = "https://localhost:6206/connect/callback-oidc",
            },
            new ClientUriEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.PostLogoutRedirect,
                Uri = "https://localhost:6206/connect/logout/callback-oidc",
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.FrontChannelLogout,
                Uri = "https://localhost:6521/connect/frontchannel-logout",
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.BackChannelLogout,
                Uri = "https://localhost:6206/connect/backchannel-logout",
            },

            // eCinema.Client
            new ClientUriEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Type = UriType.Redirect,
                Uri = "https://localhost:6204/connect/callback-oidc",
            },
            new ClientUriEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Type = UriType.PostLogoutRedirect,
                Uri = "https://localhost:6204/connect/logout/callback-oidc",
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Type = UriType.FrontChannelLogout,
                Uri = "https://localhost:6511/connect/frontchannel-logout",
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("307268b0-005c-4ee4-a0e8-a93bd0010382"),
                Type = UriType.BackChannelLogout,
                Uri = "https://localhost:6204/connect/backchannel-logout",
            }
        ];
    }
}