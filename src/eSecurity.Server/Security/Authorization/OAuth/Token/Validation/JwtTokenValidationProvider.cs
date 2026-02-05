namespace eSecurity.Server.Security.Authorization.OAuth.Token.Validation;

public class JwtTokenValidationProvider(IServiceProvider serviceProvider) : IJwtTokenValidationProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IJwtTokenValidator CreateValidator(string type)
        => _serviceProvider.GetRequiredKeyedService<IJwtTokenValidator>(type);
}