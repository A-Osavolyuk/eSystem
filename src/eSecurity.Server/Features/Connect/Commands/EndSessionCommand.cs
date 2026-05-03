using eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;

namespace eSecurity.Server.Features.Connect.Commands;

public sealed record EndSessionCommand(EndSessionRequest Request) : IRequest<Result>;

public sealed class EndSessionCommandHandler : IRequestHandler<EndSessionCommand, Result>
{
    public async Task<Result> Handle(EndSessionCommand request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}