using eSecurity.Idp.Security.Authentication.Subject.Pairwise;
using eSecurity.Idp.Security.Authentication.Subject.Public;

namespace eSecurity.Idp.Security.Authentication.Subject.Extensions;

public static class SubjectServiceCollectionExtensions
{
    public static void AddSubjects(this IServiceCollection services, Action<SubjectOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<ISubjectFactoryProvider, SubjectFactoryProvider>();
        services.AddTransient<ISubjectFactory<PublicSubjectFactoryContext>, PublicSubjectFactory>();
        services.AddTransient<ISubjectFactory<PairwiseSubjectFactoryContext>, PairwiseSubjectFactory>();
        services.AddScoped<IPairwiseSubjectQueryService, PairwiseSubjectQueryService>();
        services.AddScoped<IPairwiseSubjectCommandService, PairwiseSubjectCommandService>();
        services.AddScoped<IPublicSubjectQueryService, PublicSubjectQueryService>();
        services.AddScoped<IPublicSubjectCommandService, PublicSubjectCommandService>();
        services.AddScoped<ISubjectStrategyResolver, SubjectStrategyResolver>();
        services.AddScoped<ISubjectStrategy<PublicSubjectStrategyContext>, PublicSubjectStrategy>();
        services.AddScoped<ISubjectStrategy<PairwiseSubjectStrategyContext>, PairwiseSubjectStrategy>();
        services.AddScoped<ISubjectProvider, SubjectProvider>();
    }
}