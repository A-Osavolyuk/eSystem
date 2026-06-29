using System.Security.Claims;
using System.Text.Json;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authentication.Subject;
using eSecurity.Idp.Security.Authorization.Scopes;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Privacy;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Id;

public sealed class IdTokenFactory(
    IOptions<TokenConfigurations> options,
    ITokenBuilderProvider tokenBuilderProvider,
    ISubjectProvider subjectProvider,
    IClientQueryService clientQueryService,
    ISessionQueryService sessionQueryService,
    IScopesProcessor scopesProcessor) : ITokenFactory<IdTokenFactoryContext>
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly ISubjectProvider _subjectProvider = subjectProvider;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly ISessionQueryService _sessionQueryService = sessionQueryService;
    private readonly IScopesProcessor _scopesProcessor = scopesProcessor;

    public async ValueTask<TypedResult<string>> CreateAsync(
        IdTokenFactoryContext context,
        TokenFactoryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(context.ClientId, cancellationToken);
        List<string> scopes;

        if (options is null || options.AllowedScopes.Count == 0)
        {
            scopes = clientScopes
                .Select(x => x.Scope.Value)
                .ToList();
        }
        else
        {
            scopes = clientScopes
                .Where(x => options.AllowedScopes.Contains(x.Scope.Value))
                .Select(x => x.Scope.Value)
                .ToList();
        }

        var subjectResult = await _subjectProvider.GetSubjectAsync(
            context.UserId, context.ClientId, cancellationToken);
        
        if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
        {
            return TypedResult<string>.Fail(new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
        var tokenLifetime = context.TokenLifetime ?? _tokenConfigurations.DefaultIdTokenLifetime;
        var session = await _sessionQueryService.GetByIdAsync(context.SessionId, cancellationToken);
        if (session is null)
            throw new InvalidOperationException("Invalid session");
        
        var authenticationMethods = session.AuthenticationMethods
            .Select(x => x.MethodReference.GetString())
            .ToArray();

        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var exp = DateTimeOffset.UtcNow.Add(tokenLifetime).ToUnixTimeSeconds().ToString();
        var authTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _tokenConfigurations.Issuer),
            new(AppClaimTypes.Aud, context.ClientId.ToString()),
            new(AppClaimTypes.Sub, subject),
            new(AppClaimTypes.Sid, session.Id.ToString()),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
            new(AppClaimTypes.AuthTime, authTime, ClaimValueTypes.Integer64),
        };

        if (authenticationMethods.Length > 0)
        {
            var amrValue = JsonSerializer.Serialize(authenticationMethods);
            claims.Add(new Claim(AppClaimTypes.Amr, amrValue));
        }

        if (!string.IsNullOrEmpty(options?.Nonce))
            claims.Add(new Claim(AppClaimTypes.Nonce, options.Nonce));

        var scopeContext = new ScopeContext { UserId = context.UserId };
        var scopesProcessingResult = await _scopesProcessor.ProcessAsync(scopeContext, scopes, cancellationToken);
        if (!scopesProcessingResult.Succeeded)
        {
            var error = scopesProcessingResult.GetError();
            return TypedResult<string>.Fail(error);
        }

        var processedClaims = scopesProcessingResult.GetValue();
        claims.AddRange(processedClaims);

        var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenType.IdToken };
        var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);

        return TypedResult<string>.Success(token);
    }
}