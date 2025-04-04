using eShop.Domain.Abstraction.Exceptions;

namespace eShop.Domain.Exceptions;

public class FailedValidationException : Exception, IFailedValidationException
{
    public FailedValidationException(IEnumerable<ValidationFailure> errors, string errorMessage = "Validation error(s)") : base(errorMessage)
    {
        this.Errors = errors.Select(x => x.ErrorMessage);
    }

    public IEnumerable<string> Errors { get; }
};