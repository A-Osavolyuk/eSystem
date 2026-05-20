using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Subject.Pairwise;
using eSecurity.Idp.Security.Authentication.Subject.Public;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;

namespace eSecurity.Idp.Security.Authentication.Subject;

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