using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Seeding;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Data.Seeding;

public sealed class GrantTypeSeed : Seed<GrantTypeEntity>
{
    public override List<GrantTypeEntity> Get()
    {
        return
        [
            new GrantTypeEntity()
            {
                Id = Guid.Parse("9e90c561-4fae-40c4-b0f1-8c521131d243"),
                Grant = GrantType.AuthorizationCode
            },
            new GrantTypeEntity()
            {
                Id = Guid.Parse("fd4073e5-1bc7-41ec-8c50-3ca8f0886314"),
                Grant = GrantType.RefreshToken
            },
            new GrantTypeEntity()
            {
                Id = Guid.Parse("0ca85e15-9656-46d1-8679-121f18392acb"),
                Grant = GrantType.ClientCredentials
            },
            new GrantTypeEntity()
            {
                Id = Guid.Parse("0c88259a-f119-4fff-8680-76b8935d7920"),
                Grant = GrantType.DeviceCode
            },
            new GrantTypeEntity()
            {
                Id = Guid.Parse("f6248e9c-14e7-4c30-a89c-30b5cfcec077"),
                Grant = GrantType.TokenExchange
            },
            new GrantTypeEntity()
            {
                Id = Guid.Parse("1519a0b4-3817-40d8-acc0-d325f91ba4ab"),
                Grant = GrantType.Ciba
            },
            new GrantTypeEntity()
            {
                Id = Guid.Parse("5b080590-7410-4eab-b4a0-bc11161f030b"),
                Grant = GrantType.Implicit
            },
            new GrantTypeEntity()
            {
                Id = Guid.Parse("028e3bd4-5a4f-4eb3-8d99-af33be47fa2e"),
                Grant = GrantType.Password
            },
            new GrantTypeEntity()
            {
                Id = Guid.Parse("5b8d6839-ca69-4939-a0dd-890a51962c12"),
                Grant = GrantType.JwtBearer
            },
            new GrantTypeEntity()
            {
                Id = Guid.Parse("35283e1d-d217-4f40-90e1-9d7b1e0f1454"),
                Grant = GrantType.Saml2Bearer
            },
        ];
    }
}