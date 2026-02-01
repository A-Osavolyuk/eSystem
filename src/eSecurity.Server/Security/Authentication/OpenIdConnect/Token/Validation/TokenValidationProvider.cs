using eSystem.Core.Security.Authentication.OpenIdConnect.Token.Validation;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Validation;

public class TokenValidationProvider(IServiceProvider serviceProvider) : ITokenValidationProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenValidator CreateValidator(string type)
     => _serviceProvider.GetRequiredKeyedService<ITokenValidator>(type);
}