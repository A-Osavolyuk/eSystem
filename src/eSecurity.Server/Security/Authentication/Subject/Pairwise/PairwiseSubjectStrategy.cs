using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Subject.Pairwise;

public sealed class PairwiseSubjectStrategyContext : SubjectStrategyContext
{
    public required UserEntity User { get; set; }
    public required ClientEntity Client { get; set; }
}

public sealed class PairwiseSubjectStrategy(
    IPairwiseSubjectManager subjectManager,
    ISubjectFactoryProvider subjectFactoryProvider) : ISubjectStrategy<PairwiseSubjectStrategyContext>
{
    private readonly IPairwiseSubjectManager _subjectManager = subjectManager;
    private readonly ISubjectFactoryProvider _subjectFactoryProvider = subjectFactoryProvider;

    public async ValueTask<TypedResult<string>> ExecuteAsync(PairwiseSubjectStrategyContext context, 
        CancellationToken cancellationToken = default)
    {
        var subject = await _subjectManager.FindAsync(context.User, context.Client, cancellationToken);
        if (subject is not null) return TypedResult<string>.Success(subject.Subject);

        //TODO: Refactor salt
        const string salt = "1234567890";
        var sectorIdentifier = string.IsNullOrEmpty(context.Client.SectorIdentifierUri) 
            ? context.Client.Id.ToString() 
            : new Uri(context.Client.SectorIdentifierUri).Host;
            
        var subjectFactory = _subjectFactoryProvider.GetFactory<PairwiseSubjectFactoryContext>();
        var factoryContext = new PairwiseSubjectFactoryContext
        {
            UserId = context.User.Id, 
            SectorIdentifier = sectorIdentifier, 
            Salt =  salt
        };
        
        subject = new PairwiseSubjectEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = context.User.Id,
            ClientId = context.Client.Id,
            Subject = subjectFactory.CreateSubject(factoryContext),
            SectorIdentifier = sectorIdentifier,
        };

        var result = await _subjectManager.CreateAsync(subject, cancellationToken);
        if (result.Succeeded) return TypedResult<string>.Success(subject.Subject);
            
        var error = result.GetError();
        return TypedResult<string>.Fail(error);
    }
}