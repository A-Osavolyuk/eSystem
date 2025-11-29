using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Authentication.Oidc.Introspection;

namespace eSecurity.Server.Security.Authentication.Oidc.Introspection;

public class ReferenceTokenIntrospectionStrategy(ITokenManager tokenManager) : IIntrospectionStrategy
{
    private readonly ITokenManager _tokenManager = tokenManager;

    public async ValueTask<Result> ExecuteAsync(IntrospectionContext context, 
        CancellationToken cancellationToken = default)
    {
        var token = await _tokenManager.FindByTokenAsync(context.Token, cancellationToken);
        if (token is null || !token.IsValid) return Results.Ok(IntrospectionResponse.Fail());
        
        var tokenType = token.TokenType switch
        {
            OpaqueTokenType.AccessToken => IntrospectionTokenTypes.AccessToken,
            OpaqueTokenType.RefreshToken => IntrospectionTokenTypes.RefreshToken,
            _ => throw new NotSupportedException("Unsupported token type")
        };
        
        var response = new IntrospectionResponse()
        {
            Active = true,
            TokenType = tokenType,
            ClientId = token.Client.Id,
            Subject = token.Session.Device.UserId.ToString(),
            IssuedAt = token.CreateDate!.Value.ToUnixTimeSeconds(),
            Expiration = token.ExpiredDate.ToUnixTimeSeconds(),
            Scope = string.Join(" ", token.Scopes.Select(x => x.Scope.Name))
        };
        
        return Results.Ok(response);
    }
}