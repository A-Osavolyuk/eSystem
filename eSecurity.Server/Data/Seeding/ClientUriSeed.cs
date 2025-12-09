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
            new ClientUriEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.Redirect,
                Uri = "https://localhost:6501/connect/callback",
                CreateDate = DateTimeOffset.UtcNow,
            },
            new ClientUriEntity()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.PostLogoutRedirect,
                Uri = "https://localhost:6501/connect/logged-out",
                CreateDate = DateTimeOffset.UtcNow,
            },
            new()
            {
                Id = Guid.CreateVersion7(),
                ClientId = Guid.Parse("392e390f-33bd-4f30-af70-ccbe04bbb2c4"),
                Type = UriType.FrontChannelLogout,
                Uri = "https://localhost:6501/connect/logout/callback",
                CreateDate = DateTimeOffset.UtcNow,
            }
        ];
    }
}