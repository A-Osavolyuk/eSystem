using eSecurity.Idp.Security.Authorization.Constants;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.Validation;

namespace eSecurity.Idp.Security.Authorization.Token.Validation;

public class TokenValidationProvider(IServiceProvider serviceProvider) : ITokenValidationProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenValidator CreateValidator(TokenKind kind)
     => _serviceProvider.GetRequiredKeyedService<ITokenValidator>(kind);
}