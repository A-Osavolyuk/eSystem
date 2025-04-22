namespace eShop.Auth.Api.Utilities;

internal sealed class AppManager(
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager,
    IPermissionManager permissionManager,
    ISecurityManager securityManager,
    IProfileManager profileManager,
    ITokenManager tokenManager)
{
    public SignInManager<AppUser> SignInManager { get; } = signInManager;
    public UserManager<AppUser> UserManager { get; } = userManager;
    public RoleManager<AppRole> RoleManager { get; } = roleManager;
    public IPermissionManager PermissionManager { get; } = permissionManager;
    public ISecurityManager SecurityManager { get; } = securityManager;
    public IProfileManager ProfileManager { get; } = profileManager;
    public ITokenManager TokenManager { get; } = tokenManager;
}