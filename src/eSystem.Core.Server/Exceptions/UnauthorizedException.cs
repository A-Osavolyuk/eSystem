namespace eSystem.Core.Server.Exceptions;

public sealed class UnauthorizedException(string message) : Exception(message);