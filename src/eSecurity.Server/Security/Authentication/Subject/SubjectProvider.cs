using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.Subject.Pairwise;
using eSecurity.Server.Security.Authentication.Subject.Public;

namespace eSecurity.Server.Security.Authentication.Subject;

public sealed class SubjectProvider(
    ISubjectStrategyResolver strategyResolver) : ISubjectProvider
{
    private readonly ISubjectStrategyResolver _strategyResolver = strategyResolver;

    public async ValueTask<TypedResult<string>> GetSubjectAsync(UserEntity user, 
        ClientEntity client, CancellationToken cancellationToken = default)
    {
        if (client.SubjectType == SubjectType.Public)
        {
            var strategy = _strategyResolver.Resolver<PublicSubjectStrategyContext>();
            var context = new PublicSubjectStrategyContext() { User = user };
            return await strategy.ExecuteAsync(context, cancellationToken);
        }
        else
        {
            var strategy = _strategyResolver.Resolver<PairwiseSubjectStrategyContext>();
            var context = new PairwiseSubjectStrategyContext { User = user, Client =  client };
            return await strategy.ExecuteAsync(context, cancellationToken);
        }
    }
}