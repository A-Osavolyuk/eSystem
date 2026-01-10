namespace eSecurity.Client.Common.Confirmation;

public static class ConfirmationExtensions
{
    public static void AddConfirmation(this IServiceCollection services)
    {
        services.AddScoped<ConfirmationManager>();
    }
}