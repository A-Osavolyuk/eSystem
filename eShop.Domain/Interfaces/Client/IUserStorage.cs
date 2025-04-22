using UserModel = eShop.Domain.Models.UserModel;

namespace eShop.Domain.Interfaces.Client;

public interface IUserStorage
{
    public ValueTask<string?> GetUserNameAsync();
    public ValueTask<Guid> GetUserIdAsync();
    public ValueTask<string?> GetEmailAsync();
    public ValueTask<string?> GetPhoneNumberAsync();
    public ValueTask<UserModel?> GetUserAsync();
    public ValueTask<AccountData?> GetAccountDataAsync();
    public ValueTask SetUserAsync(UserModel model);
    public ValueTask ClearAsync();
}