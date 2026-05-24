using eSecurity.Idp.Security.Authorization.Prompt.Handlers;

namespace eSecurity.Idp.Security.Authorization.Prompt;

public static class PromptExtensions
{
    public static void AddPromptsHandling(this IServiceCollection services)
    {
        services.AddScoped<IPromptHandler, LoginPromptHandler>();
        services.AddScoped<IPromptHandler, ConsentPromptHandler>();
        services.AddScoped<IPromptHandler, SelectAccountPromptHandler>();
        services.AddScoped<IPromptHandler, NonePromptHandler>();
        services.AddTransient<IPromptStateFactory, PromptStateFactory>();
        services.AddTransient<IPromptsProcessor, PromptsProcessor>();
    }
}