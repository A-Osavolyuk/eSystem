using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Subject.Pairwise;
using eSecurity.Idp.Security.Authentication.Subject.Public;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;

namespace eSecurity.Idp.Security.Authentication.Subject;

public sealed class SubjectProvider(
    ISubjectStrategyResolver strategyResolver,
    IClientQueryService clientQueryService) : ISubjectProvider
{
    private readonly ISubjectStrategyResolver _strategyResolver = strategyResolver;
    private readonly IClientQueryService _clientQueryService = clientQueryService;

    public async ValueTask<TypedResult<string>> GetSubjectAsync(Guid userId, Guid clientId,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientQueryService.GetByIdAsync(clientId, cancellationToken);
        if (client is null)
        {
            return TypedResult<string>.Fail(new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid client"
            });
        }

        if (client.SubjectType == SubjectType.Public)
        {
            var strategy = _strategyResolver.Resolver<PublicSubjectStrategyContext>();
            var context = new PublicSubjectStrategyContext { UserId = userId };
            return await strategy.ExecuteAsync(context, cancellationToken);
        }
        else
        {
            var strategy = _strategyResolver.Resolver<PairwiseSubjectStrategyContext>();
            var context = new PairwiseSubjectStrategyContext
            {
                UserId = userId, 
                ClientId = client.Id, 
                SectorIdentifierUri = client.SectorIdentifierUri
            };
            
            return await strategy.ExecuteAsync(context, cancellationToken);
        }
    }
}