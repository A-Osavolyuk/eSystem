using eSecurity.Core.Common.Requests;

namespace eSecurity.Server.Features.Connect.Commands;

public record IntrospectionCommand(IntrospectionRequest Request) : IRequest<Result>;

public class IntrospectionCommandHandler : IRequestHandler<IntrospectionCommand, Result>
{
    public Task<Result> Handle(IntrospectionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}