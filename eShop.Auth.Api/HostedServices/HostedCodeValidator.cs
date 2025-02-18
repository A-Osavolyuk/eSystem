namespace eShop.Auth.Api.HostedServices;

public class HostedCodeValidator(IServiceScopeFactory scopeFactory) : IHostedService, IDisposable
{
    private Timer? timer;
    private readonly IServiceScopeFactory scopeFactory = scopeFactory;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new Timer(async (state) => await ValidateTokensAsync(), null, TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(15));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async Task ValidateTokensAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var codes = await context.Codes.AsNoTracking().ToListAsync();

        if (codes.Any())
        {
            var codesToRemove = new List<CodeEntity>();

            foreach (var code in codes)
            {
                if (code.ExpiresAt <= DateTime.UtcNow)
                {
                    codesToRemove.Add(code);
                }
            }

            context.Codes.RemoveRange(codesToRemove);
            await context.SaveChangesAsync();
        }
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}