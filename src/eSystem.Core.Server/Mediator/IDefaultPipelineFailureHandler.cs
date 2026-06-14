namespace eSystem.Core.Server.Mediator;

public interface IDefaultPipelineFailureHandler
{
    public Task<TResponse> HandleAsync<TResponse>(Exception exception, CancellationToken cancellationToken = default);
}