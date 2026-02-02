namespace eSecurity.Server.Common.Mapping;

public interface IAsyncMapper<in TInput, TOutput>
    where TInput : notnull
    where TOutput : notnull
{
    public Task<TOutput> MapAsync(TInput input, CancellationToken cancellationToken = default);
}