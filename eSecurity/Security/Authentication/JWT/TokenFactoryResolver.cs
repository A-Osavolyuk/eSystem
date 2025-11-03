using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Tokens;

namespace eSecurity.Security.Authentication.JWT;

public class TokenFactoryResolver(IServiceProvider serviceProvider) : ITokenFactoryResolver
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public ITokenFactory Create(JwtTokenType type) => serviceProvider.GetRequiredKeyedService<ITokenFactory>(type);
}