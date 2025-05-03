namespace eShop.Auth.Api.Data.Seed;

public class PersonalDataSeed : Seed<PersonalDataEntity, Guid>
{
    protected override List<PersonalDataEntity> Get()
    {
        return
        [
            new()
            {
                Id = Guid.Parse("dd98d543-e1bf-4835-bf76-70721dd826eb"),
                FirstName = "Alexander",
                LastName = "Osavolyuk",
                Gender = "Male",
                DateOfBirth = new DateTime(2004, 2, 11),
                UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3")
            }
        ];
    }
}