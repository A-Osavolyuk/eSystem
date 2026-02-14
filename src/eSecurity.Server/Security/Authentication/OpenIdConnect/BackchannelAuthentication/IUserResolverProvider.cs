namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public interface IUserResolverProvider
{
    public IUserResolver GetResolver(UserHint hint);
}