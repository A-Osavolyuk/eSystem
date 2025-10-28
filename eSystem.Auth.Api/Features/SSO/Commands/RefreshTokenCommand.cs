using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Features.SSO.Commands;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result>;