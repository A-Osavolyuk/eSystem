using eSystem.Core.Binding;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;
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
        var grantTypeString = form["grant_type"].ToString();
        if (string.IsNullOrEmpty(grantTypeString))
        {
            return TypedResult<TokenRequest>.Fail(new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "grant_type is mandatory"
            });
        }
        
        var grantType = EnumHelper.FromString<GrantType>(grantTypeString);
        if (grantType is null)
        {
            return TypedResult<TokenRequest>.Fail(new Error()
            {
                Code = ErrorCode.InvalidGrant,
                Description = "grant_type is invalid."
            });
        }

        return grantType.Value switch
        {
            GrantType.AuthorizationCode => await GetRequest<AuthorizationCodeRequest>(form, cancellationToken),
            GrantType.RefreshToken => await GetRequest<RefreshTokenRequest>(form, cancellationToken),
            GrantType.ClientCredentials => await GetRequest<ClientCredentialsRequest>(form, cancellationToken),
            GrantType.DeviceCode => await GetRequest<DeviceCodeRequest>(form, cancellationToken),
            GrantType.TokenExchange => await GetRequest<TokenExchangeRequest>(form, cancellationToken),
            GrantType.Ciba => await GetRequest<CibaRequest>(form, cancellationToken),
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