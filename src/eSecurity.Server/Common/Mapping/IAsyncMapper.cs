namespace eSecurity.Server.Common.Mapping;

public interface IAsyncMapper<in TInput, TOutput>
{
    public Task<TOutput> MapAsync(TInput input, CancellationToken cancellationToken = default);
}