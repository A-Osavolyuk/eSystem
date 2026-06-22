using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Idp.Features.Connect;

public record GetOpenidConfigurationQuery() : IRequest<Result>;

public class GetOpenidConfigurationQueryHandler(
    IOptions<OpenIdConfiguration> options) : IRequestHandler<GetOpenidConfigurationQuery, Result>
{
    private readonly OpenIdConfiguration _configuration = options.Value;

    public Task<Result> Handle(GetOpenidConfigurationQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Success(SuccessCodes.Ok, _configuration));
    }
}