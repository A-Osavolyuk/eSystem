using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record ExternalLoginQuery(string Provider, string? ReturnUri) : IRequest<Result>;

internal sealed class ExternalLoginQueryHandler(
    SignInManager<UserEntity> signInManager) : IRequestHandler<ExternalLoginQuery, Result>
{
    private readonly SignInManager<UserEntity> signInManager = signInManager;

    public async Task<Result> Handle(ExternalLoginQuery request,
        CancellationToken cancellationToken)
    {
        var providers = await signInManager.GetExternalAuthenticationSchemesAsync();

        var validProvider = providers.Any(x => x.DisplayName == request.Provider);

        if (!validProvider)
        {
            return Results.BadRequest($"Invalid external provider {request.Provider}.");
        }

        var handlerUri = UrlGenerator.Action("handle-external-login-response", "Security",
            new { ReturnUri = request.ReturnUri ?? "/" });
        var properties =
            signInManager.ConfigureExternalAuthenticationProperties(request.Provider, handlerUri);

        return Result.Success(new ExternalLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
    }
}