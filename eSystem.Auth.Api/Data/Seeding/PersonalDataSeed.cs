using eSystem.Core.Data.Seeding;

namespace eSystem.Auth.Api.Data.Seeding;

public class PersonalDataSeed : Seed<PersonalDataEntity>
{
    public override List<PersonalDataEntity> Get()
    {
        return
        [
            // new()
            // {
            //     Id = Guid.Parse("dd98d543-e1bf-4835-bf76-70721dd826eb"),
            //     UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
            //     FirstName = "Alexander",
            //     LastName = "Osavolyuk",
            //     Gender = Gender.Male,
            //     DateOfBirth = new DateTime(2004, 2, 11),
            // }
        ];
    }
}