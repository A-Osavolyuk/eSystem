namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record ExternalLoginQuery(string Provider, string? ReturnUri) : IRequest<Result<ExternalLoginResponse>>;

internal sealed class ExternalLoginQueryHandler(
    AppManager appManager) : IRequestHandler<ExternalLoginQuery, Result<ExternalLoginResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<ExternalLoginResponse>> Handle(ExternalLoginQuery request,
        CancellationToken cancellationToken)
    {
        var providers = await appManager.SignInManager.GetExternalAuthenticationSchemesAsync();

        var validProvider = providers.Any(x => x.DisplayName == request.Provider);

        if (!validProvider)
        {
            return new(new BadRequestException($"Invalid external provider {request.Provider}."));
        }

        var handlerUri = UrlGenerator.Action("handle-external-login-response", "Auth",
            new { ReturnUri = request.ReturnUri ?? "/" });
        var properties =
            appManager.SignInManager.ConfigureExternalAuthenticationProperties(request.Provider, handlerUri);

        return new(new ExternalLoginResponse()
        {
            Provider = request.Provider,
            AuthenticationProperties = properties
        });
    }
}