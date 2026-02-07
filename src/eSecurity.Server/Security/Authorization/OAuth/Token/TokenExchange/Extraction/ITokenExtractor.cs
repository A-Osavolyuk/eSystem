namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Extraction;

public interface ITokenExtractor<TResult> where TResult : ExtractionResult
{
    public ValueTask<TResult> ExtractAsync(string source, CancellationToken cancellationToken);
}