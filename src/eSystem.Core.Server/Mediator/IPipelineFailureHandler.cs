namespace eSystem.Core.Server.Mediator;

public interface IPipelineFailureHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<PipelineFailureResult<TResponse>> TryHandle(TRequest request, Exception exception,
        CancellationToken cancellationToken = default);
}

public sealed class PipelineFailureResult<TValue>
{
    public TValue? Value { get; set; }
    public bool Handled { get; set; }

    public static PipelineFailureResult<TValue> Return(TValue value)
        => new() { Handled = true, Value = value };

    public static PipelineFailureResult<TValue> Propagate()
        => new() { Handled = false };
}