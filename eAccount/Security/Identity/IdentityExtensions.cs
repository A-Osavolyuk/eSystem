using eAccount.Security.Identity.Users;

namespace eAccount.Security.Identity;

public static class IdentityExtensions
{
    public static void AddIdentity(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserService, UserService>();
    }
}