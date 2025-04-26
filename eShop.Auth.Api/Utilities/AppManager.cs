namespace eShop.Auth.Api.Utilities;

internal sealed class AppManager(
    SignInManager<UserEntity> signInManager,
    UserManager<UserEntity> userManager,
    RoleManager<AppRole> roleManager,
    IPermissionManager permissionManager,
    ISecurityManager securityManager,
    IProfileManager profileManager,
    ITokenManager tokenManager)
{
    public SignInManager<UserEntity> SignInManager { get; } = signInManager;
    public UserManager<UserEntity> UserManager { get; } = userManager;
    public RoleManager<AppRole> RoleManager { get; } = roleManager;
    public IPermissionManager PermissionManager { get; } = permissionManager;
    public ISecurityManager SecurityManager { get; } = securityManager;
    public IProfileManager ProfileManager { get; } = profileManager;
    public ITokenManager TokenManager { get; } = tokenManager;
}