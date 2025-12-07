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
}