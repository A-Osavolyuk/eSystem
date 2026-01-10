using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.Phone;

public interface IPhoneManager
{
    public ValueTask<List<UserPhoneNumberEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken);

    public ValueTask<List<UserPhoneNumberEntity>> GetAllAsync(UserEntity user, PhoneNumberType type,
        CancellationToken cancellationToken);

    public ValueTask<UserPhoneNumberEntity?> FindByTypeAsync(UserEntity user, PhoneNumberType type,
        CancellationToken cancellationToken);

    public ValueTask<UserPhoneNumberEntity?> FindByPhoneAsync(UserEntity user, string phone,
        CancellationToken cancellationToken);

    public ValueTask<Result> SetAsync(UserEntity user, string phoneNumber,
        PhoneNumberType type, CancellationToken cancellationToken = default);

    public ValueTask<Result> VerifyAsync(UserEntity user, string phoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetAsync(UserEntity user, string currentEmail, string newPhoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> RemoveAsync(UserEntity user, string phoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ChangeAsync(UserEntity user, string currentPhoneNumber, string newPhoneNumber,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> AddAsync(UserEntity user, string phoneNumber,
        PhoneNumberType type, CancellationToken cancellationToken = default);

    public ValueTask<bool> IsTakenAsync(string phoneNumber, CancellationToken cancellationToken = default);

    public ValueTask<bool> HasAsync(UserEntity user, PhoneNumberType type,
        CancellationToken cancellationToken = default);
}