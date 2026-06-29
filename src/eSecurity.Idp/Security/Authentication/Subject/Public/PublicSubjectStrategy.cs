using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Subject.Public;

public sealed class PublicSubjectStrategyContext : SubjectStrategyContext
{
    public required Guid UserId { get; set; }
}

public sealed class PublicSubjectStrategy(
    IPublicSubjectCommandService publicSubjectCommandService,
    IPublicSubjectQueryService publicSubjectQueryService,
    ISubjectFactoryProvider subjectFactoryProvider,
    IOptions<SubjectOptions> options) : ISubjectStrategy<PublicSubjectStrategyContext>
{
    private readonly IPublicSubjectCommandService _publicSubjectCommandService = publicSubjectCommandService;
    private readonly IPublicSubjectQueryService _publicSubjectQueryService = publicSubjectQueryService;
    private readonly ISubjectFactoryProvider _subjectFactoryProvider = subjectFactoryProvider;
    private readonly SubjectOptions _options = options.Value;

    public async ValueTask<TypedResult<string>> ExecuteAsync(PublicSubjectStrategyContext context, 
        CancellationToken cancellationToken = default)
    {
        var subject = await _publicSubjectQueryService.GetByUserAsync(context.UserId, cancellationToken);
        if (subject is not null) 
            return TypedResult<string>.Success(subject.Subject);

        var subjectFactory = _subjectFactoryProvider.GetFactory<PublicSubjectFactoryContext>();
        var factoryContext = new PublicSubjectFactoryContext { Length = _options.PublicSubjectLength };

        subject = new PublicSubjectEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = context.UserId,
            Subject = subjectFactory.CreateSubject(factoryContext)
        };

        var result = await _publicSubjectCommandService.CreateAsync(subject, cancellationToken);
        if (result.Succeeded) return TypedResult<string>.Success(subject.Subject);
            
        var error = result.GetError();
        return TypedResult<string>.Fail(error);
    }
}