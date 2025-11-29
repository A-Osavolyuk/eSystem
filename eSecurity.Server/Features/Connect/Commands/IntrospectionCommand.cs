using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Oidc.Introspection;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Authentication.Oidc.Revocation;

namespace eSecurity.Server.Features.Connect.Commands;

public record IntrospectionCommand(IntrospectionRequest Request) : IRequest<Result>;

public class IntrospectionCommandHandler(
    IIntrospectionResolver resolver,
    ITokenManager tokenManager) : IRequestHandler<IntrospectionCommand, Result>
{
    private readonly IIntrospectionResolver _resolver = resolver;
    private readonly ITokenManager _tokenManager = tokenManager;

    public async Task<Result> Handle(IntrospectionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.Token))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "token is required"
            });

        var context = new IntrospectionContext() { Token = request.Request.Token };

        if ((!string.IsNullOrEmpty(request.Request.TokenTypeHint) 
             && request.Request.TokenTypeHint == TokenTypeHints.RefreshToken) ||
            await _tokenManager.IsOpaqueAsync(request.Request.Token, cancellationToken))
        {
            var strategy = _resolver.Resolve(IntrospectionType.Reference);
            return await strategy.ExecuteAsync(context, cancellationToken);
        }
        else
        {
            var strategy = _resolver.Resolve(IntrospectionType.Jwt);
            return await strategy.ExecuteAsync(context, cancellationToken);
        }
    }
}