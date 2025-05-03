namespace eShop.Auth.Api.Data.Seed;

public class UserSeed : Seed<UserEntity>
{
    public override List<UserEntity> Get()
    {
        return
        [
            new ()
            {
                Id = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
                PersonalDataId = Guid.Parse("dd98d543-e1bf-4835-bf76-70721dd826eb"),
                Email = "sasha.osavolll111@gmail.com",
                NormalizedEmail = "sasha.osavolll111@gmail.com".ToUpper(),
                UserName = "sasha.osavolll111@gmail.com",
                NormalizedUserName = "sasha.osavolll111@gmail.com".ToUpper(),
                PasswordHash = "AQAAAAIAAYagAAAAEHeZ7iJce/rkJIBOAFdarWHCG1NUYQ1y67q5EyVGG9ttMlkXR2wxOMAQRsg+HtNtCg==",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                PhoneNumber = "380686100242",
            }
        ];
    }
}