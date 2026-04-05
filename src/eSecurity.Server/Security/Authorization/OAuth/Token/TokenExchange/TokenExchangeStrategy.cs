using System.IdentityModel.Tokens.Jwt;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;

public sealed class TokenExchangeStrategy(
    ITokenExchangeFlowResolver resolver,
    IHttpContextAccessor httpContextAccessor) : ITokenStrategy
{
    private readonly ITokenExchangeFlowResolver _resolver = resolver;
    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(TokenRequest tokenRequest,
        CancellationToken cancellationToken = default)
    {
        if (tokenRequest is not TokenExchangeRequest request)
            throw new NotSupportedException("Payload type must be 'TokenExchangeRequest'");

        if (string.IsNullOrEmpty(request.SubjectToken))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "subject_token is required"
            });
        }

        if (request.SubjectTokenType is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "subject_token_type is required"
            });
        }

        if (request.SubjectTokenType is not TokenType.AccessToken)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorCode.UnsupportedTokenType,
                Description = $"{TokenType.AccessToken.GetString()} is the only supported subject_token_type value"
            });
        }

        if (request.RequestTokenType is not TokenType.AccessToken)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorCode.UnsupportedTokenType,
                Description = $"{TokenType.AccessToken.GetString()} is the only supported request_token_type value"
            });
        }

        var clientIdClaim = _httpContext.User.FindFirst(AppClaimTypes.ClientId);
        if (clientIdClaim is null)
        {
            return Results.Unauthorized(new Error()
            {
                Code = ErrorCode.UnauthorizedClient,
                Description = "Unauthorized client"
            });
        }
        
        if (string.IsNullOrEmpty(request.Scope))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "scope is required"
            });
        }
        
        var scopes = request.Scope.Split(' ').ToList();
        if (scopes.Contains(ScopeTypes.Delegation) && scopes.Contains(ScopeTypes.Transformation))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorCode.InvalidScope,
                Description = "delegation and transformation scopes are not allowed to use in the same time"
            });
        }

        if (!scopes.Contains(ScopeTypes.Delegation) && !scopes.Contains(ScopeTypes.Transformation))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorCode.InvalidScope,
                Description = "scope must contain either delegation or transformation scope"
            });
        }

        var context = new TokenExchangeFlowContext()
        {
            ClientId = clientIdClaim.Value,
            GrantType = request.GrantType,
            SubjectToken = request.SubjectToken,
            SubjectTokenType = request.SubjectTokenType.Value,
            RequestTokenType = request.RequestTokenType,
            ActorToken = request.ActorToken,
            ActorTokenType = request.ActorTokenType,
            Audience = request.Audience,
            Scope = request.Scope
        };
        
        var tokenExchangeFlow = scopes.Contains(ScopeTypes.Delegation) 
            ? TokenExchangeFlow.Delegation 
            : TokenExchangeFlow.Transformation;
        
        var flow = _resolver.Resolve(tokenExchangeFlow);
        return await flow.ExecuteAsync(context, cancellationToken);
    }
}