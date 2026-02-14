using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public sealed class LoginHintUserResolver(IUserManager userManager) : IUserResolver
{
    private readonly IUserManager _userManager = userManager;
    
    public async Task<UserEntity?> ResolveAsync(BackchannelAuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.LoginHint)) 
            return null;
        
        return await _userManager.FindByLoginAsync(request.LoginHint, cancellationToken);
    }
}