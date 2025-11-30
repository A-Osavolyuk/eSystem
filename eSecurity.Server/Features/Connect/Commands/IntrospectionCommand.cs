using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Authentication.Oidc.Introspection;

namespace eSecurity.Server.Features.Connect.Commands;

public record IntrospectionCommand(IntrospectionRequest Request) : IRequest<Result>;

public class IntrospectionCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    IOptions<TokenOptions> options) : IRequestHandler<IntrospectionCommand, Result>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly TokenOptions _options = options.Value;

    public async Task<Result> Handle(IntrospectionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.Token))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "token is required"
            });

        var token = await _tokenManager.FindByTokenAsync(request.Request.Token, cancellationToken);
        if (token is null || !token.IsValid) return Results.Ok(IntrospectionResponse.Fail());
        
        var user = await _userManager.FindByIdAsync(token.Session.Device.UserId, cancellationToken);
        if (user is null) return Results.Ok(IntrospectionResponse.Fail());
        
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
            Issuer = _options.Issuer,
            Audience = token.Client.Audience,
            Subject = token.Session.Device.UserId.ToString(),
            Username = user.Username,
            IssuedAt = token.CreateDate!.Value.ToUnixTimeSeconds(),
            Expiration = token.ExpiredDate.ToUnixTimeSeconds(),
            Scope = string.Join(" ", token.Scopes.Select(x => x.Scope.Name))
        };
        
        return Results.Ok(response);
    }
}