using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Authorization.Token.Validation;

public class JwtTokenValidationProvider(IServiceProvider serviceProvider) : IJwtTokenValidationProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IJwtTokenValidator CreateValidator(JwtTokenType type)
        => _serviceProvider.GetRequiredKeyedService<IJwtTokenValidator>(type);
}