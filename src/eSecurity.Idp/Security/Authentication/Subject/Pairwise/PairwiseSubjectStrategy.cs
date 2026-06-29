using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Subject.Pairwise;

public sealed class PairwiseSubjectStrategyContext : SubjectStrategyContext
{
    public required Guid UserId { get; set; }
    public required Guid ClientId { get; set; }
    public string? SectorIdentifierUri { get; set; }
}

public sealed class PairwiseSubjectStrategy(
    IPairwiseSubjectQueryService pairwiseSubjectQueryService,
    IPairwiseSubjectCommandService pairwiseSubjectCommandService,
    ISubjectFactoryProvider subjectFactoryProvider,
    IOptions<SubjectOptions> options) : ISubjectStrategy<PairwiseSubjectStrategyContext>
{
    private readonly IPairwiseSubjectQueryService _pairwiseSubjectQueryService = pairwiseSubjectQueryService;
    private readonly IPairwiseSubjectCommandService _pairwiseSubjectCommandService = pairwiseSubjectCommandService;
    private readonly ISubjectFactoryProvider _subjectFactoryProvider = subjectFactoryProvider;
    private readonly SubjectOptions _options = options.Value;

    public async ValueTask<TypedResult<string>> ExecuteAsync(PairwiseSubjectStrategyContext context, 
        CancellationToken cancellationToken = default)
    {
        var subject = await _pairwiseSubjectQueryService.GetByClientAsync(context.UserId, 
            context.ClientId, cancellationToken);
        
        if (subject is not null) 
            return TypedResult<string>.Success(subject.Subject);
        
        var sectorIdentifier = string.IsNullOrEmpty(context.SectorIdentifierUri) 
            ? context.ClientId.ToString() 
            : new Uri(context.SectorIdentifierUri).Host;
            
        var subjectFactory = _subjectFactoryProvider.GetFactory<PairwiseSubjectFactoryContext>();
        var factoryContext = new PairwiseSubjectFactoryContext
        {
            UserId = context.UserId, 
            SectorIdentifier = sectorIdentifier, 
            Salt = _options.PairwiseSubjectSalt
        };
        
        subject = new PairwiseSubjectEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = context.UserId,
            ClientId = context.ClientId,
            Subject = subjectFactory.CreateSubject(factoryContext),
            SectorIdentifier = sectorIdentifier,
        };

        var result = await _pairwiseSubjectCommandService.CreateAsync(subject, cancellationToken);
        if (result.Succeeded) 
            return TypedResult<string>.Success(subject.Subject);
            
        var error = result.GetError();
        return TypedResult<string>.Fail(error);
    }
}