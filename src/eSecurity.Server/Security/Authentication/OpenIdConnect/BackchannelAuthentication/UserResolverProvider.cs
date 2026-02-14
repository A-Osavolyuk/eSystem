namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public sealed class UserResolverProvider(IServiceProvider serviceProvider) : IUserResolverProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IUserResolver GetResolver(UserHint hint)
        => _serviceProvider.GetRequiredKeyedService<IUserResolver>(hint);
}