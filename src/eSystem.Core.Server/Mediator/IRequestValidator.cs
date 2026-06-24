using eSystem.Core.Primitives;

namespace eSystem.Core.Server.Mediator;

public interface IRequestValidator<in TRequest> where TRequest : IRequest<Result>
{
    ValueTask<Result> Validate(TRequest request, CancellationToken cancellationToken);
}