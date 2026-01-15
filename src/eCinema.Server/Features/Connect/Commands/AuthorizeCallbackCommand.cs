using eCinema.Server.Security.Authentication.Oidc;
using eSystem.Core.Security.Authentication.Oidc.Client;
using MediatR;
using Microsoft.Extensions.Options;

namespace eCinema.Server.Features.Connect.Commands;

public record AuthorizeCallbackCommand : IRequest<Result>
{
    public required string? Code { get; init; }
    public required string? State { get; init; }
    public required string? Error { get; init; }
    public required string? ErrorDescription { get; init; }
}

public class AuthorizeCallbackCommandHandler(
    IOptions<ClientOptions> options,
    IOpenIdDiscoveryProvider discoveryProvider) : IRequestHandler<AuthorizeCallbackCommand, Result>
{
    private readonly IOpenIdDiscoveryProvider _discoveryProvider = discoveryProvider;
    private readonly ClientOptions _clientOptions = options.Value;

    public async Task<Result> Handle(AuthorizeCallbackCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Error) && !string.IsNullOrEmpty(request.ErrorDescription))
        {
            return Results.Found(QueryBuilder.Create()
                .WithUri("https://localhost:6511/connect/error")
                .WithQueryParam("error", request.Error)
                .WithQueryParam("error_description", request.ErrorDescription)
                .Build());
        }

        var openIdConfigurations = await _discoveryProvider.GetOpenIdConfigurationsAsync();
        
        return Results.Ok();
    }
}