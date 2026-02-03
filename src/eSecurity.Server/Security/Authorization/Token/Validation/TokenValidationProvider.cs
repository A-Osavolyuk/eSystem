using eSystem.Core.Security.Authorization.OAuth.Token.Validation;

namespace eSecurity.Server.Security.Authorization.Token.Validation;

public class TokenValidationProvider(IServiceProvider serviceProvider) : ITokenValidationProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenValidator CreateValidator(string type)
     => _serviceProvider.GetRequiredKeyedService<ITokenValidator>(type);
}