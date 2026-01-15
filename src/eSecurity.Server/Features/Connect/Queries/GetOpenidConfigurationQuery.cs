using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.Oidc;

namespace eSecurity.Server.Features.Connect.Queries;

public record GetOpenidConfigurationQuery() : IRequest<Result>;

public class GetOpenidConfigurationQueryHandler(
    IOptions<OpenIdConfiguration> options) : IRequestHandler<GetOpenidConfigurationQuery, Result>
{
    private readonly OpenIdConfiguration _configuration = options.Value;

    public Task<Result> Handle(GetOpenidConfigurationQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Ok(_configuration));
    }
}