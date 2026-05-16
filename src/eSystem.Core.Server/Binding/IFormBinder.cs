using eSystem.Core.Primitives;
using Microsoft.AspNetCore.Http;

namespace eSystem.Core.Server.Binding;

public interface IFormBinder<TOutput>
{
    public Task<TypedResult<TOutput>> BindAsync(IFormCollection form, CancellationToken cancellationToken = default);
}