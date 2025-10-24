using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.SSO.Commands;

public record GenerateTokenCommand(GenerateTokenRequest Request) : IRequest<Result>;