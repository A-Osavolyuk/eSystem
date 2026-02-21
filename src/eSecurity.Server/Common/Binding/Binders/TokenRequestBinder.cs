using eSystem.Core.Binding;
using eSystem.Core.Primitives.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Authorization.OAuth.Token.AuthorizationCode;
using eSystem.Core.Security.Authorization.OAuth.Token.Ciba;
using eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;
using eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;
using eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;
using eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;

namespace eSecurity.Server.Common.Binding.Binders;

public sealed class TokenRequestBinder(IFormBindingProvider bindingProvider) : IFormBinder<TokenRequest>
{
    private readonly IFormBindingProvider _bindingProvider = bindingProvider;

    public async Task<TypedResult<TokenRequest>> BindAsync(IFormCollection form, CancellationToken cancellationToken = default)
    {
        var grantType = form["grant_type"].ToString();
        if (string.IsNullOrEmpty(grantType))
        {
            return TypedResult<TokenRequest>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "grant_type is mandatory"
            });
        }

        return grantType switch
        {
            GrantTypes.AuthorizationCode => await GetRequest<AuthorizationCodeRequest>(form, cancellationToken),
            GrantTypes.RefreshToken => await GetRequest<RefreshTokenRequest>(form, cancellationToken),
            GrantTypes.ClientCredentials => await GetRequest<ClientCredentialsRequest>(form, cancellationToken),
            GrantTypes.DeviceCode => await GetRequest<DeviceCodeRequest>(form, cancellationToken),
            GrantTypes.TokenExchange => await GetRequest<TokenExchangeRequest>(form, cancellationToken),
            GrantTypes.Ciba => await GetRequest<CibaRequest>(form, cancellationToken),
            _ => throw new Exception("Invalid token request")
        };
    }

    private async Task<TypedResult<TokenRequest>> GetRequest<T>(IFormCollection form, 
        CancellationToken cancellationToken = default) where T : TokenRequest
    {
        var result = await _bindingProvider.GetRequiredBinder<T>().BindAsync(form, cancellationToken);
        if (!result.Succeeded || !result.TryGetValue(out var value))
        {
            var error = result.GetError();
            return TypedResult<TokenRequest>.Fail(error);
        }
        
        return TypedResult<TokenRequest>.Success(value);
    }
}