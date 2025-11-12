using eSecurity.Server.Security.Authentication.Odic.Configuration;

namespace eSecurity.Server.Features.Connect.Queries;

public record GetOpenidConfigurationQuery() : IRequest<Result>;

public class GetOpenidConfigurationQueryHandler(
    IOptions<OpenIdOptions> options) : IRequestHandler<GetOpenidConfigurationQuery, Result>
{
    private readonly OpenIdOptions _options = options.Value;

    public Task<Result> Handle(GetOpenidConfigurationQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success(_options));
    }
}