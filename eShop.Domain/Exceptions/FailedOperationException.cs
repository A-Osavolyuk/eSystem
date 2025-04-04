using eShop.Domain.Abstraction.Exceptions;

namespace eShop.Domain.Exceptions;

public class FailedOperationException(string message) : Exception(message), IInternalServerError;