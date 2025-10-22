using eShop.Domain.Common.Results;

namespace eShop.Auth.Api.Interfaces;

public interface IChangeManager
{
    public ValueTask<Result> CreateAsync(UserEntity user, ChangeField field, 
        string value, CancellationToken cancellationToken = default);
}