namespace eShop.Auth.Api.HostedServices;

public class HostedTokenValidator(IServiceScopeFactory scopeFactory) : IHostedService, IDisposable
{
    private Timer? timer;
    private readonly IServiceScopeFactory scopeFactory = scopeFactory;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new Timer(async (state) => await ValidateTokensAsync(), null, TimeSpan.FromMinutes(5),
            TimeSpan.FromHours(12));
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

        var tokens = await context.SecurityTokens.AsNoTracking().ToListAsync();
        var tokensToRemove = new List<SecurityTokenEntity>();

        foreach (var userToken in tokens)
        {
            if (userToken.ExpiredAt <= DateTime.UtcNow)
            {
                tokensToRemove.Add(userToken);
            }
        }

        context.RemoveRange(tokensToRemove);
        await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}