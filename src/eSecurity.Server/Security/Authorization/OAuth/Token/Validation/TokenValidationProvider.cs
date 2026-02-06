using eSecurity.Server.Security.Authorization.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.Validation;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.Validation;

public class TokenValidationProvider(IServiceProvider serviceProvider) : ITokenValidationProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenValidator CreateValidator(TokenKind kind)
     => _serviceProvider.GetRequiredKeyedService<ITokenValidator>(kind);
}