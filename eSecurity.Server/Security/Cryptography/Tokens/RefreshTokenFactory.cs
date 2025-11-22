using eSecurity.Server.Security.Cryptography.Keys;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class RefreshTokenContext : TokenContext
{
    public int Length { get; set; }
}

public class RefreshTokenFactory(IKeyFactory keyFactory) : ITokenFactory<RefreshTokenContext, string>
{
    private readonly IKeyFactory _keyFactory = keyFactory;

    public ValueTask<string> CreateTokenAsync(RefreshTokenContext context, CancellationToken cancellationToken = default)
    {
        var token = _keyFactory.Create(context.Length);
        return ValueTask.FromResult(token);
    }
}