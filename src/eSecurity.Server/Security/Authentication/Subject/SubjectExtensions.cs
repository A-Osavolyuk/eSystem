using eSecurity.Server.Security.Authentication.Subject.Pairwise;
using eSecurity.Server.Security.Authentication.Subject.Public;

namespace eSecurity.Server.Security.Authentication.Subject;

public static class SubjectExtensions
{
    public static void AddSubjects(this IServiceCollection services, Action<SubjectOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<ISubjectFactoryProvider, SubjectFactoryProvider>();
        services.AddTransient<ISubjectFactory<PublicSubjectFactoryContext>, PublicSubjectFactory>();
        services.AddTransient<ISubjectFactory<PairwiseSubjectFactoryContext>, PairwiseSubjectFactory>();
        services.AddScoped<IPairwiseSubjectManager, PairwiseSubjectManager>();
        services.AddScoped<IPublicSubjectManager, PublicSubjectManager>();
        services.AddScoped<ISubjectStrategyResolver, SubjectStrategyResolver>();
        services.AddScoped<ISubjectStrategy<PublicSubjectStrategyContext>, PublicSubjectStrategy>();
        services.AddScoped<ISubjectStrategy<PairwiseSubjectStrategyContext>, PairwiseSubjectStrategy>();
        services.AddScoped<ISubjectProvider, SubjectProvider>();
    }
}