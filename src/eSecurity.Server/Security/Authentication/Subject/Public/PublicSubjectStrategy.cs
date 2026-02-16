using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Subject.Public;

public sealed class PublicSubjectStrategyContext : SubjectStrategyContext
{
    public required UserEntity User { get; set; }
}

public sealed class PublicSubjectStrategy(
    IPublicSubjectManager subjectManager,
    ISubjectFactoryProvider subjectFactoryProvider,
    IOptions<SubjectOptions> options) : ISubjectStrategy<PublicSubjectStrategyContext>
{
    private readonly IPublicSubjectManager _subjectManager = subjectManager;
    private readonly ISubjectFactoryProvider _subjectFactoryProvider = subjectFactoryProvider;
    private readonly SubjectOptions _options = options.Value;

    public async ValueTask<TypedResult<string>> ExecuteAsync(PublicSubjectStrategyContext context, 
        CancellationToken cancellationToken = default)
    {
        var subject = await _subjectManager.FindAsync(context.User, cancellationToken);
        if (subject is not null) return TypedResult<string>.Success(subject.Subject);

        var subjectFactory = _subjectFactoryProvider.GetFactory<PublicSubjectFactoryContext>();
        var factoryContext = new PublicSubjectFactoryContext() { Length = _options.PublicSubjectLength };

        subject = new PublicSubjectEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = context.User.Id,
            Subject = subjectFactory.CreateSubject(factoryContext)
        };

        var result = await _subjectManager.CreateAsync(subject, cancellationToken);
        if (result.Succeeded) return TypedResult<string>.Success(subject.Subject);
            
        var error = result.GetError();
        return TypedResult<string>.Fail(error);
    }
}