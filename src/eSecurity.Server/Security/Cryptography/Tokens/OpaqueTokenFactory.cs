using eSecurity.Server.Security.Cryptography.Keys;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class OpaqueTokenContext : TokenContext
{
    public int Length { get; set; }
}

public class OpaqueTokenFactory(IKeyFactory keyFactory) : ITokenFactory<OpaqueTokenContext, string>
{
    private readonly IKeyFactory _keyFactory = keyFactory;

    public ValueTask<string> CreateTokenAsync(OpaqueTokenContext context, CancellationToken cancellationToken = default)
    {
        var token = _keyFactory.Create(context.Length);
        return ValueTask.FromResult(token);
    }
}