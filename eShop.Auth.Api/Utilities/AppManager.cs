namespace eShop.Auth.Api.Utilities;

internal sealed class AppManager(
    SignInManager<UserEntity> signInManager,
    UserManager<UserEntity> userManager,
    RoleManager<RoleEntity> roleManager,
    IPermissionManager permissionManager,
    ISecurityManager securityManager,
    IProfileManager profileManager)
{
    public readonly SignInManager<UserEntity> SignInManager = signInManager;
    public readonly UserManager<UserEntity> UserManager = userManager;
    public readonly RoleManager<RoleEntity> RoleManager = roleManager;
    public readonly IPermissionManager PermissionManager = permissionManager;
    public readonly ISecurityManager SecurityManager = securityManager;
    public readonly IProfileManager ProfileManager = profileManager;
}