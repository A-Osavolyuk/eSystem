namespace eSecurity.Idp.Security.Authentication.BackchannelAuthentication;

public interface IUserResolverProvider
{
    public IUserResolver GetResolver(UserHint hint);
}