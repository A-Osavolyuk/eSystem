using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.Oidc.Introspection;
using eSystem.Core.Security.Authentication.Oidc.Revocation;

namespace eSecurity.Server.Features.Connect.Commands;

public record IntrospectionCommand(IntrospectionRequest Request) : IRequest<Result>;

public class IntrospectionCommandHandler(
    IIntrospectionResolver resolver) : IRequestHandler<IntrospectionCommand, Result>
{
    private readonly IIntrospectionResolver _resolver = resolver;

    public async Task<Result> Handle(IntrospectionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.Token))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "token is required"
            });
        
        var context = new IntrospectionContext(){ Token = request.Request.Token };
        var strategy = _resolver.Resolve(request.Request.TokenTypeHint ?? TokenTypeHints.AccessToken);
        return await strategy.ExecuteAsync(context, cancellationToken);
    }
}