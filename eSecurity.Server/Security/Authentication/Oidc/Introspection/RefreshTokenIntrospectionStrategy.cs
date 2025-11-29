using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Authentication.Oidc.Introspection;

namespace eSecurity.Server.Security.Authentication.Oidc.Introspection;

public class RefreshTokenIntrospectionStrategy(ITokenManager tokenManager) : IIntrospectionStrategy
{
    private readonly ITokenManager _tokenManager = tokenManager;

    public async ValueTask<Result> ExecuteAsync(IntrospectionContext context, 
        CancellationToken cancellationToken = default)
    {
        var token = await _tokenManager.FindByTokenAsync(context.Token, cancellationToken);
        if (token is null || token.Revoked || !token.IsValid) return Results.Ok(IntrospectionResponse.Fail());

        var scope = string.Join(" ", token.Client.AllowedScopes.Select(x => x.Scope.Name));
        var response = new IntrospectionResponse()
        {
            Active = true,
            TokenType = IntrospectionTokenTypes.RefreshToken,
            ClientId = token.Client.Id,
            Subject = token.Session.Device.UserId.ToString(),
            IssuedAt = token.CreateDate!.Value.ToUnixTimeSeconds(),
            Expiration = token.ExpireDate.ToUnixTimeSeconds(),
            Scope = scope
        };
        
        return Results.Ok(response);
    }
}