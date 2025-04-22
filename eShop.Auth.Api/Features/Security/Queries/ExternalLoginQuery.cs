using eShop.Domain.Common.API;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record ExternalLoginQuery(string Provider, string? ReturnUri) : IRequest<Result>;

internal sealed class ExternalLoginQueryHandler(
    AppManager appManager) : IRequestHandler<ExternalLoginQuery, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(ExternalLoginQuery request,
        CancellationToken cancellationToken)
    {
        var providers = await appManager.SignInManager.GetExternalAuthenticationSchemesAsync();

        var validProvider = providers.Any(x => x.DisplayName == request.Provider);

        if (!validProvider)
        {
            return Results.BadRequest($"Invalid external provider {request.Provider}.");
        }

        var handlerUri = UrlGenerator.Action("handle-external-login-response", "Auth",
            new { ReturnUri = request.ReturnUri ?? "/" });
        var properties =
            appManager.SignInManager.ConfigureExternalAuthenticationProperties(request.Provider, handlerUri);

        return Result.Success(new ExternalLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
    }
}