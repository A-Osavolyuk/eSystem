namespace eSystem.Core.Server.Exceptions;

public sealed class ValidationException(string message) : Exception(message);