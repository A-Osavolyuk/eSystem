namespace eShop.Auth.Api.Utilities;

internal sealed class AppManager(
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IPermissionManager permissionManager,
    ISecurityManager securityManager,
    IProfileManager profileManager)
{
    public readonly SignInManager<AppUser> SignInManager = signInManager;
    public readonly UserManager<AppUser> UserManager = userManager;
    public readonly RoleManager<IdentityRole> RoleManager = roleManager;
    public readonly IPermissionManager PermissionManager = permissionManager;
    public readonly ISecurityManager SecurityManager = securityManager;
    public readonly IProfileManager ProfileManager = profileManager;
}