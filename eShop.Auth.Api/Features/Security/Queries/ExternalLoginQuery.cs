using eShop.Domain.Responses.API.Auth;
using Microsoft.AspNetCore.Authentication;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record ExternalLoginQuery(string Provider, string? ReturnUri) : IRequest<Result>;

internal sealed class ExternalLoginQueryHandler(
    ISignInManager signInManager) : IRequestHandler<ExternalLoginQuery, Result>
{
    private readonly ISignInManager signInManager = signInManager;

    public async Task<Result> Handle(ExternalLoginQuery request,
        CancellationToken cancellationToken)
    {
        var providers = await signInManager.GetExternalAuthenticationSchemasAsync(cancellationToken);
        var validProvider = providers.Any(x => x == request.Provider);

        if (!validProvider)
        {
            return Results.BadRequest($"Invalid external provider {request.Provider}.");
        }

        var redirectUrl = UrlGenerator.Action("handle-external-login-response", "Security",
            new { ReturnUri = request.ReturnUri ?? "/" });
        
        var properties = signInManager.ConfigureExternalAuthenticationProperties(request.Provider, redirectUrl);
        
        var result = Result.Success(new ExternalLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
        
        return await Task.FromResult(result);
    }
}