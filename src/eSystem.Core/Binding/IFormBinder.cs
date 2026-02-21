using eSystem.Core.Primitives.Results;
using Microsoft.AspNetCore.Http;

namespace eSystem.Core.Binding;

public interface IFormBinder<TOutput>
{
    public Task<TypedResult<TOutput>> BindAsync(IFormCollection form, CancellationToken cancellationToken = default);
}