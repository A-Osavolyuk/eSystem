using eShop.Domain.Responses.API.Auth;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record ExternalLoginQuery(string Provider, string? ReturnUri) : IRequest<Result>;

internal sealed class ExternalLoginQueryHandler() : IRequestHandler<ExternalLoginQuery, Result>
{
    public async Task<Result> Handle(ExternalLoginQuery request,
        CancellationToken cancellationToken)
    {
        List<string> providers = ["Google", "Microsoft", "Twitter", "Facebook"];
        var validProvider = providers.Any(x => x == request.Provider);

        if (!validProvider)
        {
            return Results.BadRequest($"Invalid external provider {request.Provider}.");
        }
        
        var handlerUri = UrlGenerator.Action("handle-external-login-response", "Security", new { ReturnUri = request.ReturnUri ?? "/" });
        
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = handlerUri,
            Items =
            {
                ["LoginProvider"] = request.Provider,
                ["XsrfId"] = Guid.NewGuid().ToString(),
            }
        };

        return Result.Success(new ExternalLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
    }
}