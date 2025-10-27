using System.Security.Claims;
using eSystem.Auth.Api.Security.Authentication.SSO.Session;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Claims;

namespace eSystem.Auth.Api.Features.SSO.Commands;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result>;