namespace eShop.Auth.Api.Extensions;

public static class RoleManagerExtensions
{
    public static async Task<IEnumerable<RoleData>?> GetRolesDataAsync<TRole>(this RoleManager<TRole> roleManager,
        IEnumerable<string> roles) where TRole : IdentityRole<Guid>
    {
        var rolesList = roles.ToList();
        if (!rolesList.Any())
        {
            return null;
        }

        var rolesInfo = new List<RoleData>();

        foreach (var role in rolesList)
        {
            var roleInfo = await roleManager.FindByNameAsync(role);

            rolesInfo.Add(new RoleData()
            {
                Id = roleInfo!.Id,
                Name = roleInfo.Name!,
                NormalizedName = roleInfo.NormalizedName!
            });
        }

        return rolesInfo;
    }
}